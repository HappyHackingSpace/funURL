package cli

import (
	"bufio"
	"fmt"
	"net/url"
	"os"
	"strings"

	"github.com/spf13/cobra"
)

var encodeCmd = &cobra.Command{
	Use:   "encode",
	Short: "URL-encode strings",
	Run: func(cmd *cobra.Command, args []string) {
		urlString, _ := cmd.Flags().GetString("url")
		component, _ := cmd.Flags().GetBool("component")
		doubled, _ := cmd.Flags().GetBool("double")

		if urlString != "" {
			encodeURL(urlString, component, doubled)
		} else if len(args) > 0 {
			encodeURL(args[0], component, doubled)
		} else {
			scanner := bufio.NewScanner(os.Stdin)
			if scanner.Scan() {
				input := strings.TrimSpace(scanner.Text())
				encodeURL(input, component, doubled)
			} else {
				fmt.Println("Error: No input provided")
				os.Exit(1)
			}
		}
	},
}

func init() {
	rootCmd.AddCommand(encodeCmd)
	encodeCmd.Flags().BoolP("component", "c", false, "Use QueryEscape instead of PathEscape (for query components)")
	encodeCmd.Flags().BoolP("double", "d", false, "Use double encoding")
}

func encodeURL(input string, component bool, doubled bool) {
	var encoded string

	if component {
		encoded = url.QueryEscape(input)
	} else if doubled {
		encoded = url.QueryEscape(url.QueryEscape(input))
	} else {
		encoded = url.PathEscape(input)
	}

	fmt.Println(encoded)
}
