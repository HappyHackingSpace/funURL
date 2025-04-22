package cli

import (
	"bufio"
	"fmt"
	"net/url"
	"os"
	"strings"

	"github.com/spf13/cobra"
)

var decodeCmd = &cobra.Command{
	Use:   "decode",
	Short: "URL-decode strings",
	Run: func(cmd *cobra.Command, args []string) {
		urlString, _ := cmd.Flags().GetString("url")
		component, _ := cmd.Flags().GetBool("component")
		if urlString != "" {
			decodeURL(urlString, component)
		} else if len(args) > 0 {
			decodeURL(args[0], component)
		} else {
			scanner := bufio.NewScanner(os.Stdin)
			if scanner.Scan() {
				input := strings.TrimSpace(scanner.Text())
				decodeURL(input, component)
			} else {
				fmt.Println("Error: No input provided")
				os.Exit(1)
			}
		}
	},
}

func init() {
	rootCmd.AddCommand(decodeCmd)
	decodeCmd.Flags().BoolP("component", "c", false, "Use QueryUnescape instead of PathUnescape (for query components)")
}

func decodeURL(input string, component bool) {
	var decoded string
	var err error
	if component {
		decoded, err = url.QueryUnescape(input)
	} else {
		decoded, err = url.PathUnescape(input)
	}
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error decoding URL: %v\n", err)
		os.Exit(1)
	}
	fmt.Println(decoded)
}
