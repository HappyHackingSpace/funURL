#!/bin/bash

# Color definitions
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color
BOLD='\033[1m'

# Make sure funURL is in PATH
if ! command -v funURL &> /dev/null; then
    echo -e "${RED}Error: funURL not found in PATH${NC}"
    echo "Please install funURL first or make sure it's in your PATH"
    exit 1
fi

# Function to run a command and display its output
run_command() {
    local cmd="$1"
    local description="$2"
    
    echo -e "\n${BLUE}${BOLD}Test:${NC} ${description}"
    echo -e "${YELLOW}Command:${NC} ${cmd}"
    echo -e "${GREEN}Output:${NC}"
    
    # Execute the command
    eval "$cmd"
    
    local exit_code=$?
    if [ $exit_code -ne 0 ]; then
        echo -e "\n${RED}Command failed with exit code $exit_code${NC}"
    else
        echo -e "\n${GREEN}Command executed successfully${NC}"
    fi
    
    # Separator for readability
    echo -e "${BLUE}----------------------------------------${NC}"
}

echo -e "${BOLD}URL Parsing Tests${NC}"

# Parse all URL components
run_command "funURL parse https://subdomain.vulnerabletarget.com/path?query=value#fragment" \
    "Parse all URL components"

# Extract specific components
run_command "funURL parse -c https://vulnerabletarget.com" \
    "Extract protocol/scheme"
    
run_command "funURL parse -s https://subdomain.vulnerabletarget.com" \
    "Extract subdomain"
    
run_command "funURL parse -t https://vulnerabletarget.com" \
    "Extract top-level domain"
    
run_command "funURL parse -n https://vulnerabletarget.com" \
    "Extract hostname"
    
run_command "funURL parse -p https://vulnerabletarget.com/path" \
    "Extract path"
    
run_command "funURL parse -q 'https://vulnerabletarget.com/?key=value'" \
    "Extract query parameters"
    
run_command "funURL parse -f 'https://vulnerabletarget.com/#section'" \
    "Extract fragments"

echo -e "${BOLD}URL Modification Tests${NC}"

# Change protocol
run_command "funURL modify -c https http://vulnerabletarget.com" \
    "Change protocol"

# Update path
run_command "funURL modify -p /new/path https://vulnerabletarget.com/old/path" \
    "Update path"

# Change query string
run_command "funURL modify -q 'key1=value1&key2=value2' 'https://vulnerabletarget.com?old=param'" \
    "Change query string"

# Update fragment
run_command "funURL modify -f 'new-section' 'https://vulnerabletarget.com#old-section'" \
    "Update fragment"

echo -e "${BOLD}URL Encoding Tests${NC}"

# Path-encode a URL
run_command "funURL encode 'hello world'" \
    "Path-encode a URL"

# Query component encoding
run_command "funURL encode -c 'param=value with spaces'" \
    "Query component encoding"

echo -e "${BOLD}URL Decoding Tests${NC}"

# Path-decode a URL
run_command "funURL decode 'hello%20world'" \
    "Path-decode a URL"

# Query component decoding
run_command "funURL decode -c 'param%3Dvalue%20with%20spaces'" \
    "Query component decoding"

echo -e "${BOLD}Input Method Tests${NC}"

# Via command-line argument
run_command "funURL parse https://vulnerabletarget.com" \
    "Via command-line argument"

# Via the --url flag
run_command "funURL parse --url https://vulnerabletarget.com" \
    "Via the --url flag"

# Via standard input (pipe)
run_command "echo 'https://vulnerabletarget.com' | funURL parse" \
    "Via standard input (pipe)"

echo -e "${BOLD}Example Tests${NC}"

# Parse a URL and extract all components
run_command "funURL parse https://user:pass@sub.vulnerabletarget.com:8080/path/to/page?query=string#fragment" \
    "Parse a URL with all components"

# Encode a query parameter
run_command "funURL encode -c 'search=special chars: &?=+'" \
    "Encode a query parameter with special characters"

# Decode a path component
run_command "funURL decode 'my%20encoded%20path'" \
    "Decode a path component"

# Change protocol and path in one command
run_command "funURL modify -c https -p /new/path http://vulnerabletarget.com/old" \
    "Change protocol and path in one command"

echo -e "\n${BOLD}${GREEN}All tests completed!${NC}"