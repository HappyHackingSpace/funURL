APP_NAME := funURL
CLI_PROJECT := funURL.CLI
TEST_PROJECT := funURL.CLI.Tests
CONFIGURATION := Release

.PHONY: build test run clean publish restore install uninstall

build:
	dotnet build $(CLI_PROJECT) -c $(CONFIGURATION)

test:
	dotnet test $(TEST_PROJECT) -c $(CONFIGURATION)

run:
	dotnet run --project $(CLI_PROJECT) -- $(ARGS)

clean:
	dotnet clean
	rm -rf artifacts

restore:
	dotnet restore

publish:
	dotnet publish $(CLI_PROJECT) -c $(CONFIGURATION) -o artifacts
