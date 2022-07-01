# XivReClassPlugin
ReClass.NET Plugin to make use of the info gathered in [FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs)

just throw the dll into ```ReClass.Net\x64\Plugins\``` and set the path to ```FFXIVClientStructs\ida\data.yml``` in the settings.

simulates having RTTI info available with the things defined in data.yml

also adds a slightly improved C# Code Generator to generate strucs that are more suitable for FFXIVClientStructs usage
and resolves instance names to addresses where available (e.g. `<Client::Game::Object::GameObjectManager_Instance>`
