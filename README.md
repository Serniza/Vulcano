# Vulcano

## Unity Game Developer Toolkit

A set of tools and utilities created to speed up the development of my own Unity games. The goal is to improve productivity, code organization, and project extensibility.

### Singletons Manager

A system that manages MonoBehaviour or Interface Singletons as project-wide services.

- Simple integration
- Controlled global access
- Designed for modular architecture

How to use it?

You can drag any GameObject into the component.

![Singletons Manager 1](https://github.com/user-attachments/assets/a85f2b86-21d8-41a5-baaf-1d6714109a53)

You can select any MonoBehaviour or interface from that component.

![Singletons Manager 2](https://github.com/user-attachments/assets/47186383-5461-41f7-86aa-735ab214d592)

When the game starts, all references are automatically registered in Awake (important).

![Singletons Manager 3](https://github.com/user-attachments/assets/f29d20a1-200e-40ad-9e81-ac754be1532c)

Once registered, you can retrieve them whenever you need like this:

![Singletons Manager 4](https://github.com/user-attachments/assets/531e3df4-0d76-4cd8-895c-30b9b9253ff6)

Or if it is not a MonoBehaviour:

![Singletons Manager 5](https://github.com/user-attachments/assets/224d6afb-ae4f-4dd6-9b05-52afd92370d0)

Additionally, there is a base class you can inherit from that automatically registers the MonoBehaviour in its Awake method.

![Singletons Manager 5](https://github.com/user-attachments/assets/4559e31e-c352-48c5-855a-f88f5e87f59e)

### Panels Manager

A UI management system responsible for organizing and controlling game panels and popups.

- Handles panel activation and deactivation using Unity's SetActive method. Popup panels are managed through scene loading and unloading.
- Centralized UI flow control
- Cleaner interface management

### Foldout Attribute

A custom Unity Inspector attribute that allows grouping variables into collapsible sections.

Example usage:

![Foldout 1](https://github.com/user-attachments/assets/141103d7-90c9-4bb5-9b92-fd2f0c31a279)

![Foldout 2](https://github.com/user-attachments/assets/b276f5f5-d13e-4204-a48d-dae74651dcb8)

![Foldout 3](https://github.com/user-attachments/assets/4c0d682d-e7db-4810-a3a6-0c12eb907e35)

![Foldout 4](https://github.com/user-attachments/assets/103b8cc9-781e-441d-93f3-6d69aee03179)

![Foldout 5](https://github.com/user-attachments/assets/cd0b8cbb-0a10-4a80-844e-c699cec276f7)

![Foldout 6](https://github.com/user-attachments/assets/4e16dad3-0e33-4ae5-a383-17e5e101eef3)

⚠️ Note: using // inside the name may break the Inspector visualization.

<b>Circle Transform</b>

A tool for positioning and moving objects in a circular layout using:

- Radius
- Angle

Useful for:

- Radial distributions
- Orbiting objects
- Procedural layouts

https://github.com/user-attachments/assets/d2e4e06f-d8fc-4fe7-86ce-49c14c8aeb4a

### Json Handler

Utility for parsing JSON data into dictionary-like structures.

- Dynamic data access
- Unique script
