#addin nuget:?package=Cake.Android.Adb&version=3.2.0
#addin nuget:?package=Cake.Android.AvdManager&version=2.2.0

DirectoryPath ROOT_PATH = MakeAbsolute(Directory("../.."));

#load "shared.cake"

var TEST_APP = Argument("app", EnvironmentVariable("ANDROID_TEST_APP") ?? "");
var TEST_RESULTS = Argument("results", EnvironmentVariable("ANDROID_TEST_RESULTS") ?? "");
var TEST_DEVICE = Argument("device", EnvironmentVariable("ANDROID_TEST_DEVICE") ?? "android-emulator-64");
var TEST_VERSION = Argument("deviceVersion", EnvironmentVariable("ANDROID_TEST_DEVICE_VERSION") ?? "34");
var TEST_APP_PACKAGE_NAME = Argument("package", EnvironmentVariable("ANDROID_TEST_APP_PACKAGE_NAME") ?? "");
var TEST_APP_INSTRUMENTATION = Argument("instrumentation", EnvironmentVariable("ANDROID_TEST_APP_INSTRUMENTATION") ?? "devicerunners.xharness.maui.XHarnessInstrumentation");

// other
var ANDROID_AVD = "DEVICE_TESTS_EMULATOR";
var DEVICE_NAME = Argument("skin", EnvironmentVariable("ANDROID_TEST_SKIN") ?? "Nexus 5X");
var DEVICE_ID = "";
var DEVICE_ARCH = "";

// set up env
var ANDROID_SDK_ROOT = Argument("android", EnvironmentVariable("ANDROID_SDK_ROOT") ?? EnvironmentVariable("ANDROID_SDK_HOME"));
if (string.IsNullOrEmpty(ANDROID_SDK_ROOT)) {
    throw new Exception("Environment variable 'ANDROID_SDK_ROOT' must be set to the Android SDK root.");
}
System.Environment.SetEnvironmentVariable("PATH",
    $"{ANDROID_SDK_ROOT}/cmdline-tools/latest/bin" + System.IO.Path.PathSeparator +
    $"{ANDROID_SDK_ROOT}/platform-tools" + System.IO.Path.PathSeparator +
    $"{ANDROID_SDK_ROOT}/emulator" + System.IO.Path.PathSeparator +
    $"{ANDROID_SDK_ROOT}/tools/bin" + System.IO.Path.PathSeparator +
    EnvironmentVariable("PATH"));

Information("Android SDK Root: {0}", ANDROID_SDK_ROOT);
Information("PATH: {0}", EnvironmentVariable("PATH"));

var bat = IsRunningOnWindows() ? ".bat" : "";
var exe = IsRunningOnWindows() ? ".exe" : "";
var avdSettings = new AndroidAvdManagerToolSettings {
    SdkRoot = ANDROID_SDK_ROOT,
    ToolPath = $"{ANDROID_SDK_ROOT}/cmdline-tools/latest/bin/avdmanager{bat}"
};
var adbSettings = new AdbToolSettings {
    SdkRoot = ANDROID_SDK_ROOT,
    ToolPath = $"{ANDROID_SDK_ROOT}/platform-tools/adb{exe}"
};
var emuSettings = new AndroidEmulatorToolSettings {
    SdkRoot = ANDROID_SDK_ROOT,
    ToolPath = $"{ANDROID_SDK_ROOT}/emulator/emulator{exe}",
    ArgumentCustomization = args => args.Append("-no-window")
};

AndroidEmulatorProcess emulatorProcess = null;

Setup(context =>
{
    if (!string.IsNullOrEmpty(TEST_VERSION) && TEST_VERSION != "latest")
        TEST_DEVICE = $"{TEST_DEVICE}_{TEST_VERSION}";

    Information("Test App: {0}", TEST_APP);
    Information("Test Device: {0}", TEST_DEVICE);
    Information("Test Results Directory: {0}", TEST_RESULTS);

    // determine the device characteristics
    {
        var working = TEST_DEVICE.Trim().ToLower();
        var emulator = true;
        var api = 34;
        // version
        if (working.IndexOf("_") is int idx && idx > 0) {
            api = int.Parse(working.Substring(idx + 1));
            working = working.Substring(0, idx);
        }
        var parts = working.Split('-');
        // os
        if (parts[0] != "android")
            throw new Exception("Unexpected platform (expected: android) in device: " + TEST_DEVICE);
        // device/emulator
        if (parts[1] == "device")
            emulator = false;
        else if (parts[1] != "emulator" && parts[1] != "simulator")
            throw new Exception("Unexpected device type (expected: device|emulator) in device: " + TEST_DEVICE);
        // arch/bits
        if (parts[2] == "32") {
            if (emulator)
                DEVICE_ARCH = "x86";
            else
                DEVICE_ARCH = "armeabi-v7a";
        } else if (parts[2] == "64") {
            if (RuntimeInformation.ProcessArchitecture == System.Runtime.InteropServices.Architecture.Arm64)
                DEVICE_ARCH = "arm64-v8a";
            else if (emulator)
                DEVICE_ARCH = "x86_64";
            else
                DEVICE_ARCH = "arm64-v8a";
        }
        DEVICE_ID = $"system-images;android-{api};google_apis;{DEVICE_ARCH}";

        // we are not using a virtual device, so quit
        if (!emulator)
            return;
    }

    Information("Test Device ID: {0}", DEVICE_ID);

    // delete the AVD first, if it exists
    Information("Deleting AVD if exists: {0}...", ANDROID_AVD);
    try { AndroidAvdDelete(ANDROID_AVD, avdSettings); }
    catch { }

    // create the new AVD
    Information("Creating AVD: {0}...", ANDROID_AVD);
    AndroidAvdCreate(ANDROID_AVD, DEVICE_ID, DEVICE_NAME, force: true, settings: avdSettings);

    // start the emulator
    Information("Starting Emulator: {0}...", ANDROID_AVD);
    emulatorProcess = AndroidEmulatorStart(ANDROID_AVD, emuSettings);

    // wait for it to finish booting (10 mins)
    var waited = 0;
    var total = 60 * 10;
    while (AdbShell("getprop sys.boot_completed", adbSettings).FirstOrDefault() != "1") {
        System.Threading.Thread.Sleep(1000);
        Information("Wating {0}/{1} seconds for the emulator to boot up.", waited, total);
        if (waited++ > total)
            break;
    }
    Information("Waited {0} seconds for the emulator to boot up.", waited);
});

Teardown(context =>
{
    // no virtual device was used
    if (emulatorProcess == null)
        return;

    // stop and cleanup the emulator
    AdbEmuKill(adbSettings);

    System.Threading.Thread.Sleep(5000);

    // kill the process if it has not already exited
    try { emulatorProcess.Kill(); }
    catch { }

    // delete the AVD
    try { AndroidAvdDelete(ANDROID_AVD, avdSettings); }
    catch { }
});

Task("Default")
    .Does(() =>
{
    if (string.IsNullOrEmpty(TEST_APP)) {
        throw new Exception("A path to a test app is required.");
    }
    if (string.IsNullOrEmpty(TEST_APP_PACKAGE_NAME)) {
        var appFile = (FilePath)TEST_APP;
        appFile = appFile.GetFilenameWithoutExtension();
        TEST_APP_PACKAGE_NAME = appFile.FullPath.Replace("-Signed", "");
    }
    if (string.IsNullOrEmpty(TEST_APP_INSTRUMENTATION)) {
        TEST_APP_INSTRUMENTATION = TEST_APP_PACKAGE_NAME + ".TestInstrumentation";
    }
    if (string.IsNullOrEmpty(TEST_RESULTS)) {
        TEST_RESULTS = TEST_APP + "-results";
    }

    Information("Test App: {0}", TEST_APP);
    Information("Test App Package Name: {0}", TEST_APP_PACKAGE_NAME);
    Information("Test App Instrumentation: {0}", TEST_APP_INSTRUMENTATION);
    Information("Test Results Directory: {0}", TEST_RESULTS);

    CleanDirectories(TEST_RESULTS);

    DotNetTool("xharness android test " +
        $"--app=\"{TEST_APP}\" " +
        $"--package-name=\"{TEST_APP_PACKAGE_NAME}\" " +
        $"--instrumentation=\"{TEST_APP_INSTRUMENTATION}\" " +
        $"--output-directory=\"{TEST_RESULTS}\" " +
        $"--verbosity=\"Debug\" ");

    var failed = XmlPeek($"{TEST_RESULTS}/TestResults.xml", "/assemblies/assembly[@failed > 0 or @errors > 0]/@failed");
    if (!string.IsNullOrEmpty(failed)) {
        throw new Exception($"At least {failed} test(s) failed.");
    }
});

RunTarget(TARGET);
