BINARY_NAME=funURL
INSTALL_DIR=/usr/local/bin
USER_BIN=$(HOME)/.local/bin
OS=$(shell uname -s)

.PHONY: all build clean

all: banner build

banner:
	@echo "\033[0;32m"
	@echo "   __           _    _ ___ _    "
	@echo "  / _|_  _ _ _ | |  | | _ \ |   "
	@echo " |  _| || | ' \\| |__| |   / |__ "
	@echo " |_|  \\_,_|_||_|____|_|_|_\\____|"
	@echo "\033[0m"
	@echo "A Swiss Army Knife for URLs"
	@echo ""

build:
	@echo "Building $(BINARY_NAME) for $(OS)..."
ifeq ($(OS), Windows_NT)
	GOOS=windows GOARCH=amd64 go build -o $(BINARY_NAME).exe cmd/main.go
else ifeq ($(OS), Linux)
	GOOS=linux GOARCH=amd64 go build -o $(BINARY_NAME) cmd/main.go
else ifeq ($(OS), Darwin)
	GOOS=darwin GOARCH=amd64 go build -o $(BINARY_NAME) cmd/main.go
else
	@echo "Unsupported OS: $(OS)"
	exit 1
endif

clean:
	@echo "Cleaning up..."
ifeq ($(OS), Windows_NT)
	@rm -f $(BINARY_NAME).exe
else
	@rm -f $(BINARY_NAME)
endif
	@echo "Done."
