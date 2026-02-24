# PersonalERP

## Cursor Cloud specific instructions

### Overview

PersonalERP is a modular client-server ERP system built on .NET Framework 4.8 (C#, WCF, WinForms). On Linux, it is built and run using **Mono**. See `README.md` for module creation instructions.

### Architecture

- **PersonalERP_Server** (console app): Hosts two WCF endpoints and loads plugin modules from `Modules/` directory
- **PersonalERP_Client** (WinForms): Connects to server, downloads and runs modules (requires GUI)
- **PERP_API / PERP_CommLibrary**: Shared service contract libraries
- **PERP_DatabaseBrowser / PersonalERP_Accounting**: Example plugin modules (DLLs)

### Build

```
nuget restore PersonalERP.sln
msbuild PersonalERP.sln /p:Configuration=Debug
```

### Linux/Mono gotchas

1. **Case-sensitivity**: The repo has `app.config` (lowercase) but `.csproj` files reference `App.config`. Symlinks are needed:
   ```
   ln -sf app.config PersonalERP/App.config
   ln -sf app.config PersonalERP_Client/App.config
   ```

2. **libdl.so**: On Ubuntu 24.04, `libdl` is merged into `libc`. Mono's SQLitePCLRaw requires a `libdl.so` symlink:
   ```
   sudo ln -sf /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so
   ```

3. **Native SQLite**: The repo ships `libe_sqlite3.so` for Linux under `PersonalERP/runtimes/linux-x64/native/`. Copy it to the build output:
   ```
   cp PersonalERP/runtimes/linux-x64/native/libe_sqlite3.so PersonalERP/bin/Debug/
   ```

4. **WCF behavior config**: Mono's WCF requires explicit `behaviorConfiguration` on `<service>` elements. The build output `PersonalERP.exe.config` must be patched to add a `<behaviors>` section with a named `<behavior>` and reference it from each `<service>` element. See the update script or run steps below for the exact patch.

### Running the server

```
cd PersonalERP/bin/Debug
sleep infinity | mono PersonalERP.exe &
```

The `sleep infinity` pipe keeps stdin open so `Console.ReadLine()` blocks and the server stays alive.

- WCF Comm endpoint: `http://localhost:3740/endpoint`
- WCF API endpoint: `http://localhost:3443/endpoint`

### Testing endpoints

```
curl -s -X POST http://localhost:3740/endpoint \
  -H "Content-Type: text/xml; charset=utf-8" \
  -H "SOAPAction: http://tempuri.org/IPERP_CommModel/ListModules" \
  -d '<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"><s:Body><ListModules xmlns="http://tempuri.org/" /></s:Body></s:Envelope>'
```

### Notes

- The `PERP_API.dll` in the Modules directory is correctly flagged as "Invalid module" (it's a library, not a module plugin).
- The server fetches its external IP from `http://icanhazip.com` on startup; this requires internet access.
- There are no automated tests in this repository.
- There is no linter configured for this repository.
- The CI workflow (`.github/workflows/dotnet.yml`) is non-functional as `setup-dotnet` doesn't support .NET Framework 4.8.
