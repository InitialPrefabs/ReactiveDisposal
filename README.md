# Reactive Disposal #

Reactive Disposal is a small utility to help manage unmanaged/unsafe memory in Unity's Entity Component System. This 
takes advantage of `ISystemStateComponentData` alongside `IComponentData` on unmanaged memory.

For the difference between the two, please take a look [here](https://docs.unity3d.com/Packages/com.unity.entities@0.0/manual/system_state_components.html).

## Use Cases ##
Because `ISystemStateComponentData` have different lifetimes compared to `IComponentData`, all unmanaged memory must be 
disposed manually.

While it is completely possible to store unmanaged memory in `MonoBehaviour`s, this greatly couples the use of objects 
with something that is honestly just pure data. This also makes the project very messy as objects which can be converted 
and destroyed into their entity format, are now injected and persistent thus taking up more memory within the game.

## Install Guide ##

### As a submodule ###

Simply clone this project as a submodule into your git repository.

```
git submodule add https://github.com/InitialPrefabs/ReactiveDisposal.git <path-to-desired-directory>
```
### As a Unity Package
Add this line to `Packages/manifest.json`

* `"com.initialprefabs.reactive-disposal": "git://github.com/InitialPrefabs/ReactiveDisposal.git"`

## Docs ##
Jump over to the [wiki](https://github.com/InitialPrefabs/ReactiveDisposal/wiki) for more detailed info!

## General TODOs

* [x] Implement unit tests on all reactive systems
* [x] Implement unit tests on all blob data structures
* [ ] Implement play mode tests so that there are some safety integration tests no runtime
* [x] Write wiki
* [x] Add package support
