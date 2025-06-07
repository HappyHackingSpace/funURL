package cli

import (
	"HappyHackingSpace/funURL/internal/utils"
	"bufio"
	"fmt"
	"os"
	"strings"

	"github.com/jedib0t/go-pretty/v6/table"
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
	tbl := table.NewWriter()
	tbl.SetStyle(table.StyleLight)
	tbl.SetOutputMirror(os.Stdout)
	tbl.AppendHeader(table.Row{"Component", "Value"})

	queryTbl := table.NewWriter()
	queryTbl.SetStyle(table.StyleLight)
	queryTbl.SetOutputMirror(os.Stdout)
	queryTbl.AppendHeader(table.Row{"Query", "Value"})

	parsedUrl := utils.CreateURL(urlString)

	if !protocol && !subdomain && !tld && !hostname && !path && !query && !fragments {
		protocol, subdomain, tld, hostname, path, query, fragments = true, true, true, true, true, true, true
	}

	if protocol {
		tbl.AppendRow(table.Row{"Protocol", utils.StringOrDefault(parsedUrl.Scheme)})
	}

	if subdomain {
		host := parsedUrl.Hostname()
		parts := strings.Split(host, ".")
		if len(parts) > 2 {
			tbl.AppendRow(table.Row{"Subdomain", strings.Join(parts[:len(parts)-2], ".")})
		} else {
			tbl.AppendRow(table.Row{"Subdomain", utils.StringOrDefault("")})
		}
	}

	if tld {
		host := parsedUrl.Hostname()
		parts := strings.Split(host, ".")
		if len(parts) >= 2 {
			tbl.AppendRow(table.Row{"TLD", parts[len(parts)-1]})

		} else {
			tbl.AppendRow(table.Row{"TLD", utils.StringOrDefault("")})
		}
	}

	if hostname {
		tbl.AppendRow(table.Row{"Hostname", utils.StringOrDefault(parsedUrl.Hostname())})
	}

	if path {
		tbl.AppendRow(table.Row{"Path", utils.StringOrDefault(parsedUrl.Path)})
	}

	if query {
		queryParams := parsedUrl.Query()
		if len(queryParams) > 0 {
			tbl.AppendRow(table.Row{"Query Params", fmt.Sprintf("%v value", len(queryParams))})
			for key, values := range queryParams {
				for _, value := range values {
					queryTbl.AppendRow(table.Row{key, utils.StringOrDefault(value)})
				}
			}
		} else {
			tbl.AppendRow(table.Row{"Query Params", utils.StringOrDefault("")})
		}
	}

	if fragments {
		tbl.AppendRow(table.Row{"Fragments", utils.StringOrDefault(parsedUrl.Fragment)})
	}

	tbl.Render()

	if query && queryTbl.Length() > 0 {
		queryTbl.Render()
	}
}
