<img src="https://madewithlove.now.sh/af?text=Happy+Hackking+Space" alt="Made with love in Happy Hackking Space">

# funURL

<img src="img/image.png"> <br>
funURL is a command-line tool designed as a Swiss Army knife for URLs. It takes a URL as input (via stdin, command-line flags, or arguments) and provides a range of operations for parsing, validation, modification, encoding/decoding, and handling URLs efficiently.

## Requirements

- Golang 1.18+

## Installation
`funURL` requires go1.18+ to install successfully. Run the following command to get the repo:
```
go install -v github.com/HappyHackingSpace/funURL/cmd/funurl@main
```

## Features

### URL Parsing

Extract specific components from a URL:

```bash
# Parse all URL components
funURL parse https://subdomain.vulnerabletarget.com/path?query=value#fragment

# Extract specific components
funURL parse -c https://vulnerabletarget.com     # Extract protocol/scheme
funURL parse -s https://subdomain.vulnerabletarget.com    # Extract subdomain
funURL parse -t https://vulnerabletarget.com     # Extract top-level domain
funURL parse -n https://vulnerabletarget.com     # Extract hostname
funURL parse -p https://vulnerabletarget.com/path    # Extract path
funURL parse -q https://vulnerabletarget.com/?key=value    # Extract query parameters
funURL parse -f https://vulnerabletarget.com/#section    # Extract fragments
```

### URL Modification

Add, remove, or update URL components:

```bash
# Change protocol
funURL modify -c https http://vulnerabletarget.com

# Update path
funURL modify -p /new/path https://vulnerabletarget.com/old/path

# Change query string
funURL modify -q "key1=value1&key2=value2" https://vulnerabletarget.com?old=param

# Update fragment
funURL modify -f "new-section" https://vulnerabletarget.com#old-section
```

### URL Encoding

Encode URLs or URL components:

```bash
# Path-encode a URL
funURL encode "hello world"

# Query component encoding
funURL encode -c "param=value with spaces"
```

### URL Decoding

Decode URLs or URL components:

```bash
# Path-decode a URL
funURL decode "hello%20world"

# Query component decoding
funURL decode -c "param%3Dvalue%20with%20spaces"
```

### Deduping Urls

```bash
funURL dedupe https://google.com https://google.com/home?qs=value https://google.com/home?qs=secondValue https://google.com/home?qs=newValue&secondQs=anotherValue https://google.com/home?qs=asd&secondQs=das https://site.com/api/users/123 https://site.com/api/users/222 https://site.com/api/users/412/profile https://site.com/users/photos/photo.jpg https://site.com/users/photos/myPhoto.jpg https://site.com/users/photos/photo.png https://google.com/home/?q=2&d=asd https://my.site/profile?param1=1&param2=2 https://my.site/profile?param3=3

cat urls.txt | funURL dedupe
```

## Input Methods

funURL supports multiple ways to provide input:

1. Via command-line argument:
   ```bash
   funURL parse https://vulnerabletarget.com
   ```

2. Via the `--url` flag:
   ```bash
   funURL parse --url https://vulnerabletarget.com
   ```

3. Via standard input (pipe or interactive):
   ```bash
   echo "https://vulnerabletarget.com" | funURL parse
   # Or
   funURL parse  # Then type the URL and press Enter
   ```

## Examples

```bash
# Parse a URL and extract all components
funURL parse https://user:pass@sub.vulnerabletarget.com:8080/path/to/page?query=string#fragment

# Encode a query parameter
funURL encode -c "search=special chars: &?=+"

# Decode a path component
funURL decode "my%20encoded%20path"

# Change protocol and path in one command
funURL modify -c https -p /new/path http://vulnerabletarget.com/old
```

## Contributing

Hack! don't forget to follow [CONTRIBUTING](./CONTRIBUTING)

---

<img src="img/hhs.svg" alt="Made with love in Happy Hackking Space">
