## Toolbox.Core
A new and improved library to eventually be used by the current Switch Toolbox making development significantly easier.

## Features
- Libraries for GUI completely seperated.
- Everything works as a dotnet standard library and can be ported to any cross platform target (browser, mac/linux, android, etc)
- Improved plugin system. Plugins now automatically get interfaces for file formats and other various use cases.
- Plugin loading improved. Any dll with IPlugin interface will load as one.
- Models now use an IModelFormat interface to easily obtain the renderer and generic model. 
- Models now default to a generic renderer for quickly adding new model formats.

## Todo
- Organize better.
- Add DDS encoding methods.
- Add sample plugins.
- Improve code more.

## Credits

- Ploaj for a base on the DAE writer.
- JuPaHe64 for the base 3D renderer.
- Every File Explorer devs (Gericom) for Yaz0 and bitmap font stuff.
- exelix for BYAML, SARC and KCL library.
- Syroot for helpful IO extensions and libraries.
- GDKChan for DDS decode methods.
- AboodXD for some foundation stuff with exelix's SARC library, Wii U (GPU7) and Switch (Tegra X1) textures swizzling, reading/converting uncompressed types for DDS, and documentation for GTX, XTX, and BNTX. Library for Yaz0 made by AboodXD and helped port it to the tool.
- Sage of Mirrors for texture decoding. 
- Ambrosia for BTI and TXE support.
- Kuriimu for some IO and file parsing help.
- Skyth and Radfordhound for PAC documentation.
- Ac_K for ASTC decoder c# port from Ryujinx. 
- pkNX and kwsch for Fnv hashing and useful pkmn code/structure references.
- Dragonation for useful code on the structure for some flatbuffers in pokemon switch
- mvit and Rei for help with gfpak hash strings and also research for formats.
- QuickBMS for some compression code ported (LZ77 WII)
