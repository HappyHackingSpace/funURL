package cli

import (
	"bufio"
	"fmt"
	"iter"
	"net/url"
	"os"
	"strings"

	"github.com/HappyHackingSpace/funURL/internal/utils"
	"github.com/spf13/cobra"
)

var dedupeCmd = &cobra.Command{
	Use:   "dedupe",
	Short: "Dedupe urls",
	Run: func(cmd *cobra.Command, args []string) {
		var urls []*url.URL
		seen := make(map[string]bool)

		if len(args) > 0 {
			for _, value := range args {
				if value == "" {
					continue
				}
				url := utils.CreateURL(value)
				urlStr := url.String()
				if !seen[urlStr] {
					seen[urlStr] = true
					urls = append(urls, url)
				}
			}
		} else {
			scanner := bufio.NewScanner(os.Stdin)
			for scanner.Scan() {
				value := scanner.Text()
				if value == "" {
					continue
				}
				url := utils.CreateURL(value)
				urlStr := url.String()
				if !seen[urlStr] {
					seen[urlStr] = true
					urls = append(urls, url)
				}
			}
		}

		if len(urls) == 0 {
			fmt.Println("Error: No input provided")
			os.Exit(1)
			return
		}

		// Convert slice to iterator
		urlIterator := func(yield func(*url.URL) bool) {
			for _, u := range urls {
				if !yield(u) {
					return
				}
			}
		}

		dedupeUrls(urlIterator)
	},
}

func init() {
	rootCmd.AddCommand(dedupeCmd)
}

func dedupeUrls(urls iter.Seq[*url.URL]) {
	seen := make(map[string]*url.URL)
	var result []*url.URL

	for u := range urls {
		base := u.Scheme + "://" + u.Host

		pathParts := strings.Split(strings.Trim(u.Path, "/"), "/")
		var normalizedParts []string

		for _, part := range pathParts {
			if part == "" {
				continue
			}

			if isNumeric(part) || strings.Contains(part, ".") {
				normalizedParts = append(normalizedParts, "{param}")
			} else {
				normalizedParts = append(normalizedParts, part)
			}
		}

		normalizedPath := "/" + strings.Join(normalizedParts, "/")

		params := u.Query()
		var paramKeys []string
		for key := range params {
			paramKeys = append(paramKeys, key)
		}

		for i := 0; i < len(paramKeys); i++ {
			for j := i + 1; j < len(paramKeys); j++ {
				if paramKeys[i] > paramKeys[j] {
					paramKeys[i], paramKeys[j] = paramKeys[j], paramKeys[i]
				}
			}
		}

		signature := base + normalizedPath + "|"
		for _, key := range paramKeys {
			signature += key + ","
		}

		if _, exists := seen[signature]; !exists {
			seen[signature] = u
			result = append(result, u)
		}
	}

	for _, u := range result {
		fmt.Println(u.String())
	}
}

func isNumeric(s string) bool {
	for _, char := range s {
		if char < '0' || char > '9' {
			return false
		}
	}
	return len(s) > 0
}
