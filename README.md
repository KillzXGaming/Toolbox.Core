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
- Add DDS encoding methods.
- Add sample plugins.
- Improve code more.
