#!/bin/bash

set -e

# Color codes for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Default installation directory
INSTALL_DIR="/usr/local/bin"
BINARY_NAME="funURL"

# Print banner
echo -e "${GREEN}"
echo "   __           _    _ ___ _    "
echo "  / _|_  _ _ _ | |  | | _ \ |   "
echo " |  _| || | ' \| |__| |   / |__ "
echo " |_|  \_,_|_||_|____|_|_|_\____|"
echo -e "${NC}"
echo "A Swiss Army Knife for URLs"
echo ""

# Check if Go is installed
if ! command -v go &> /dev/null; then
    echo -e "${RED}Error: Go is not installed or not in PATH${NC}"
    echo "Please install Go first: https://golang.org/doc/install"
    exit 1
fi

# Check if running from repository directory by looking for expected structure
if [ ! -f "go.mod" ] || [ ! -d "cmd" ] || [ ! -d "internal/cli" ]; then
    echo -e "${RED}Error: Important project files not found.${NC}"
    echo "This script should be run from the funURL repository root."
    exit 1
fi

echo -e "\n${GREEN}Building funURL from source...${NC}"
echo "Compiling application..."
go build -o $BINARY_NAME cmd/main.go || { echo -e "${RED}Failed to build funURL${NC}"; exit 1; }

echo -e "\n${YELLOW}Select installation method:${NC}"
echo "1) Install to system directory ($INSTALL_DIR) - may require sudo"
echo "2) Install to local user bin directory (~/.local/bin)"
echo "3) Keep executable in current directory"
read -p "Select option [1-3]: " install_option

case $install_option in
    1)
        echo "Installing to $INSTALL_DIR (may require sudo)..."
        sudo mv $BINARY_NAME "$INSTALL_DIR/" || { echo -e "${RED}Failed to install funURL${NC}"; exit 1; }
        echo -e "${GREEN}funURL has been installed to $INSTALL_DIR${NC}"
        ;;
        
    2)
        USER_BIN="$HOME/.local/bin"
        # Create directory if it doesn't exist
        mkdir -p "$USER_BIN" || { echo -e "${RED}Failed to create $USER_BIN${NC}"; exit 1; }
        
        # Move executable
        mv $BINARY_NAME "$USER_BIN/" || { echo -e "${RED}Failed to install funURL${NC}"; exit 1; }
        
        # Check if directory is in PATH
        if [[ ":$PATH:" != *":$USER_BIN:"* ]]; then
            echo -e "${YELLOW}$USER_BIN is not in your PATH.${NC}"
            echo "Add the following line to your ~/.bashrc or ~/.zshrc:"
            echo "export PATH=\"\$PATH:$USER_BIN\""
            
            # Offer to add it automatically
            read -p "Add to PATH automatically? (y/N): " add_to_path
            if [[ "$add_to_path" =~ ^[Yy]$ ]]; then
                shell_file="$HOME/.bashrc"
                # Check if user is using zsh
                if [[ "$SHELL" == *"zsh"* ]]; then
                    shell_file="$HOME/.zshrc"
                fi
                
                echo "export PATH=\"\$PATH:$USER_BIN\"" >> "$shell_file"
                echo -e "${GREEN}Added $USER_BIN to PATH in $shell_file${NC}"
                echo "Please restart your terminal or run 'source $shell_file' to apply changes"
            fi
        fi
        
        echo -e "${GREEN}funURL has been installed to $USER_BIN${NC}"
        ;;
        
    3)
        echo -e "${GREEN}funURL has been built in the current directory.${NC}"
        echo "You can run it with: ./$BINARY_NAME"
        ;;
        
    *)
        echo -e "${RED}Invalid option. Keeping executable in current directory.${NC}"
        echo "You can run it with: ./$BINARY_NAME"
        ;;
esac

# Display post-installation message
echo -e "\n${GREEN}Thank you for installing funURL!${NC}"
echo "For help, run: funURL --help"