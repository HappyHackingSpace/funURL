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
		if urlString != "" {
			encodeURL(urlString, component)
		} else if len(args) > 0 {
			encodeURL(args[0], component)
		} else {
			scanner := bufio.NewScanner(os.Stdin)
			if scanner.Scan() {
				input := strings.TrimSpace(scanner.Text())
				encodeURL(input, component)
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
}

func encodeURL(input string, component bool) {
	var encoded string

	if component {
		encoded = url.QueryEscape(input)
	} else {
		encoded = url.PathEscape(input)
	}

	fmt.Println(encoded)
}
