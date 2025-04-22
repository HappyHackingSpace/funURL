package cli

import (
	"HappyHackingSpace/funURL/internal/utils"
	"bufio"
	"fmt"
	"os"
	"strings"

	"github.com/spf13/cobra"
)

var parseCmd = &cobra.Command{
	Use:   "parse",
	Short: "Extract protocol, domain, path, query parameters, and fragments",
	Run: func(cmd *cobra.Command, args []string) {
		urlString, _ := cmd.Flags().GetString("url")
		protocol, _ := cmd.Flags().GetBool("protocol")
		subdomain, _ := cmd.Flags().GetBool("subdomain")
		tld, _ := cmd.Flags().GetBool("tld")
		hostname, _ := cmd.Flags().GetBool("hostname")
		path, _ := cmd.Flags().GetBool("path")
		query, _ := cmd.Flags().GetBool("query")
		fragments, _ := cmd.Flags().GetBool("fragments")

		if urlString != "" {
			extractUrl(urlString, protocol, subdomain, tld, hostname, path, query, fragments)
		} else if len(args) > 0 {
			extractUrl(args[0], protocol, subdomain, tld, hostname, path, query, fragments)
		} else {
			scanner := bufio.NewScanner(os.Stdin)
			if scanner.Scan() {
				input := strings.TrimSpace(scanner.Text())
				extractUrl(input, protocol, subdomain, tld, hostname, path, query, fragments)
			} else {
				fmt.Println("Error: No input provided")
				os.Exit(1)
			}
		}
	}}

func init() {
	rootCmd.AddCommand(parseCmd)
	parseCmd.Flags().StringP("url", "u", "", "URL to parse")
	parseCmd.Flags().BoolP("protocol", "c", false, "parse scheme / protocol")
	parseCmd.Flags().BoolP("subdomain", "s", false, "parse subdomain")
	parseCmd.Flags().BoolP("tld", "t", false, "parse top level domain")
	parseCmd.Flags().BoolP("hostname", "n", false, "parse hostname")
	parseCmd.Flags().BoolP("path", "p", false, "parse path")
	parseCmd.Flags().BoolP("query", "q", false, "parse query string")
	parseCmd.Flags().BoolP("fragments", "f", false, "parse fragments")
}

func extractUrl(urlString string, protocol, subdomain, tld, hostname, path, query, fragments bool) {
	parsedUrl := utils.CreateURL(urlString)

	if !protocol && !subdomain && !tld && !hostname && !path && !query && !fragments {
		protocol, subdomain, tld, hostname, path, query, fragments = true, true, true, true, true, true, true
	}

	if protocol {
		fmt.Printf("Protocol: %s\n", parsedUrl.Scheme)
	}

	if subdomain {
		host := parsedUrl.Hostname()
		parts := strings.Split(host, ".")
		if len(parts) > 2 {
			fmt.Printf("Subdomain: %s\n", strings.Join(parts[:len(parts)-2], "."))
		}
	}

	if tld {
		host := parsedUrl.Hostname()
		parts := strings.Split(host, ".")
		if len(parts) >= 2 {
			fmt.Printf("TLD: %s\n", parts[len(parts)-1])
		}
	}

	if hostname {
		fmt.Printf("Hostname: %s\n", parsedUrl.Hostname())
	}

	if path {
		fmt.Printf("Path: %s\n", parsedUrl.Path)
	}

	if query {
		queryParams := parsedUrl.Query()
		if len(queryParams) > 0 {
			var formattedParams []string
			for key, values := range queryParams {
				for _, value := range values {
					formattedParams = append(formattedParams, fmt.Sprintf("%s=%s", key, value))
				}
			}
			fmt.Printf("Query Params: %s\n", strings.Join(formattedParams, " "))
		}
	}

	if fragments {
		fmt.Printf("Fragments: %s\n", parsedUrl.Fragment)
	}
}
