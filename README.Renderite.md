# Running Renderite + FrooxEngine on macOS

It is possible to patch Renderite and FrooxEngine to use macOS
primitives for semaphore synchronization and override some of the Linux-specific
code that activates when Renderite detects it's running in Wine. In
particular:

- CrossOver does not natively work because of a bug in its
  implementation of `mfreadwrite.dll`, which is fixed in Proton. Copying
  the DLL from such an installation works because it's a PE DLL. A copy
  is included in the repo; put it in the bottle's System32 and also
  under CrossOver. Place the DLL in
```
/Applications/CrossOver.app/Contents/SharedSupport/CrossOver/lib/wine/x86_64-windows
```
  and also under the bottle's System32 folder.

- Many functions in Resonite and Renderite assume that Wine == Linux,
  and so use linux-specific code. Patched versions of Interprocess and Wine-ShmBridge
  are included in the repo. Sources are available at
  https://github.com/nullobsi/Wine-ShmBridge and
  https://github.com/nullobsi/interprocess. For the new version of
  Wine-ShmBridge to be loaded, the WINE DLL Path is adjusted.

  Copy `renderite_binaries/{engine,renderer}.YellowDogMan.Cloudtoid.Interprocess.dll` to `resonite/` and `resonite/Renderer/Renderite.Renderer_Data/Managed`
  respectively. Then, copy both files `renderite_binaries/shmbridge.{dll,so}` to `resonite/Renderer/`.

- Finally, the renderer uses a Linux-specific interface to determine if
  the engine is still running. A patched version of Renderite.Unity is
  in the repository and sources are available at
  https://github.com/nullobsi/Renderite which disables the check
  entirely, and fixes the loading of shader asset bundles on CrossOver.

  Copy `renderite_binaries/Renderite.Unity.dll` to
  `resonite/Renderer/Renderite.Renderer_Data/Managed`.
