#!/bin/bash

# Base directory
BASE_DIR="./"

# Create main directories
mkdir -p "$BASE_DIR"/{Core/{Interfaces,Events},Models/{Data,Configuration},Views/{UI/{Components,Screens},Animations},Controllers,Services,Utils}

# Create interface files
touch "$BASE_DIR/Core/Interfaces/IDataService.cs"
touch "$BASE_DIR/Core/Interfaces/IGameState.cs"
touch "$BASE_DIR/Core/Interfaces/IUIState.cs"

# Create events file
touch "$BASE_DIR/Core/Events/GameEvents.cs"

# Create model files
touch "$BASE_DIR/Models/Data/Trial.cs"
touch "$BASE_DIR/Models/Data/Metadata.cs"
touch "$BASE_DIR/Models/Data/DataPipeBody.cs"
touch "$BASE_DIR/Models/Configuration/GameConfig.cs"
touch "$BASE_DIR/Models/GameData.cs"

# Create view files
touch "$BASE_DIR/Views/UI/Components/ScoreDisplay.cs"
touch "$BASE_DIR/Views/UI/Components/TrialProgressBar.cs"
touch "$BASE_DIR/Views/UI/Screens/MainGameUI.cs"
touch "$BASE_DIR/Views/UI/Screens/GameOverUI.cs"
touch "$BASE_DIR/Views/Animations/UIAnimationController.cs"

# Create controller files
touch "$BASE_DIR/Controllers/GameController.cs"
touch "$BASE_DIR/Controllers/DataController.cs"
touch "$BASE_DIR/Controllers/UIController.cs"

# Create service files
touch "$BASE_DIR/Services/DataService.cs"
touch "$BASE_DIR/Services/WebService.cs"

# Create utility files
touch "$BASE_DIR/Utils/Constants.cs"
touch "$BASE_DIR/Utils/Utility.cs"

# Print the created structure
echo "Created folder structure:"
ls -R "$BASE_DIR"