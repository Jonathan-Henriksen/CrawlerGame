# NeuralJourney

## Project Description

NeuralJourney is an innovative multiplayer gaming experience that seamlessly integrates OpenAI's GPT model to interpret and execute player commands. Through a unique combination of traditional game mechanics and cutting-edge AI technology, players can navigate the game world using natural language inputs. The project encapsulates various components including game logic, server-client architecture, and AI-driven interactions, offering an old-school interactive adventure.

## OpenAI Implementation

NeuralJourney uses OpenAI's GPT model to interpret player commands. The player's input and available commands are sent as a prompt to OpenAI, formatted as `"{availableCommands}\n\n{userInput}\n\n###\n\n"`. The completion end sequences are defined by the stop sequence `StopSequence` option in the OpenAI configuration. This integration allows for a dynamic mapping of player inputs to in-game actions, enhancing the gameplay experience.

## Strategy and Programming Patterns

1. **Service-Oriented Architecture (SOA):** NeuralJourney employs a service-oriented architecture, modularizing functionalities into distinct services like `OpenAIService`. This promotes reusability, maintainability, and scalability within the application.

2. **Factory Pattern:** The command processing utilizes a factory pattern, as seen in `CommandFactory`, to create command objects based on player input. This abstracts the command creation process, allowing for flexibility and extensibility in handling various commands.

3. **Singleton Pattern:** Certain services, such as the clock service, may be implemented as singletons to ensure a single instance throughout the application, maintaining consistency and efficiency.

4. **Command Pattern:** The game's actions are encapsulated in command objects, enabling a decoupling of the sender and receiver objects. This pattern enhances the flexibility in executing different commands and can facilitate undo/redo functionalities.

5. **Dependency Injection:** Dependencies like `OpenAIOptions` are injected into classes such as `OpenAIService`, enhancing testability and adherence to the Dependency Inversion Principle.

6. **Enum-Based Command Handling:** Commands are represented as enums (`CommandEnum`), providing a type-safe way to define and handle various game commands. This approach simplifies command processing and ensures robustness.

7. **Asynchronous Programming:** Asynchronous methods are used for handling player inputs and communicating with the OpenAI API, ensuring a responsive and efficient user experience.

8. **Semaphores for Thread Safety:** Semaphores are utilized to make stream writing thread-safe, ensuring that concurrent write operations to the same stream are handled correctly. This synchronization mechanism is vital for maintaining data integrity and preventing race conditions.
