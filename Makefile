APP_NAME := funURL
VERSION := $(shell git describe --tags --always --dirty 2>/dev/null || echo "dev")
COMMIT := $(shell git rev-parse --short HEAD 2>/dev/null || echo "unknown")
DATE := $(shell date -u +"%Y-%m-%dT%H:%M:%SZ")

CMD_DIR := ./cmd
MAIN_FILE := $(CMD_DIR)/main.go

LDFLAGS := -ldflags "-X main.version=$(VERSION) -X main.commit=$(COMMIT) -X main.date=$(DATE) -s -w"

BUILD_DIR := build

PLATFORMS := \
	darwin/amd64 \
	darwin/arm64 \
	linux/amd64 \
	linux/arm64 \
	linux/386 \
	windows/amd64 \
	windows/386

.PHONY: all clean build test platforms help install dev-deps

all: clean test build

build:
	@echo "Building $(APP_NAME) for current platform..."
	@mkdir -p $(BUILD_DIR)
	$(eval OUTPUT=$(BUILD_DIR)/$(APP_NAME))
	$(eval ifeq ($(shell go env GOOS),windows)
		$(eval OUTPUT := $(OUTPUT).exe)
	endif)
	go build $(LDFLAGS) -o $(OUTPUT) $(CMD_DIR)

platforms: clean
	@echo "Building $(APP_NAME) for all platforms..."
	@mkdir -p $(BUILD_DIR)
	$(foreach PLATFORM,$(PLATFORMS), \
		$(call build_platform,$(PLATFORM)))

define build_platform
	$(eval GOOS=$(word 1,$(subst /, ,$(1))))
	$(eval GOARCH=$(word 2,$(subst /, ,$(1))))
	$(eval OUTPUT=$(BUILD_DIR)/$(APP_NAME)-$(GOOS)-$(GOARCH))
	@echo "Building for $(GOOS)/$(GOARCH)..."
	@if [ "$(GOOS)" = "windows" ]; then \
		GOOS=$(GOOS) GOARCH=$(GOARCH) go build $(LDFLAGS) -o $(OUTPUT).exe $(CMD_DIR); \
	else \
		GOOS=$(GOOS) GOARCH=$(GOARCH) go build $(LDFLAGS) -o $(OUTPUT) $(CMD_DIR); \
	fi
endef

test:
	@echo "Running tests..."
	go test -v ./...

clean:
	@echo "Cleaning build directory..."
	@rm -rf $(BUILD_DIR)

deps:
	@echo "Installing dependencies..."
	go mod download
	go mod tidy

fmt:
	@echo "Formatting code..."
	go fmt ./...

lint:
	@echo "Linting code..."
	golangci-lint run

run:
	go run $(CMD_DIR) $(ARGS)

install:
	@echo "Installing $(APP_NAME) to GOPATH/bin..."
	go install $(LDFLAGS) $(CMD_DIR)

dev-deps:
	@echo "Installing development dependencies..."
	@command -v golangci-lint >/dev/null 2>&1 || { \
		echo "Installing golangci-lint..."; \
		go install github.com/golangci/golangci-lint/cmd/golangci-lint@latest; \
	}

help:
	@echo "Available targets:"
	@echo "  all        - Clean, test, and build for current platform"
	@echo "  build      - Build for current platform"
	@echo "  platforms  - Build for all platforms"
	@echo "  test       - Run tests"
	@echo "  clean      - Clean build directory"
	@echo "  deps       - Install dependencies"
	@echo "  fmt        - Format code"
	@echo "  lint       - Lint code"
	@echo "  run        - Run the application (use ARGS='--name=John' for arguments)"
	@echo "  install    - Install binary to GOPATH/bin"
	@echo "  dev-deps   - Install development dependencies"
	@echo "  help       - Show this help"
