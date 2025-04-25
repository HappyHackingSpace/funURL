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
		doubled, _ := cmd.Flags().GetBool("double")
		if urlString != "" {
			decodeURL(urlString, component, doubled)
		} else if len(args) > 0 {
			decodeURL(args[0], component, doubled)
		} else {
			scanner := bufio.NewScanner(os.Stdin)
			if scanner.Scan() {
				input := strings.TrimSpace(scanner.Text())
				decodeURL(input, component, doubled)
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
	decodeCmd.Flags().BoolP("double", "d", false, "Use double decoding")
}

func decodeURL(input string, component bool, doubled bool) {
	var decoded string
	var err error
	if doubled {
		// First decode
		decoded, err = url.QueryUnescape(input)
		if err != nil {
			fmt.Fprintf(os.Stderr, "Error decoding URL (1st pass): %v\n", err)
			os.Exit(1)
		}
		// Second decode
		decoded, err = url.QueryUnescape(decoded)
		if err != nil {
			fmt.Fprintf(os.Stderr, "Error decoding URL (2nd pass): %v\n", err)
			os.Exit(1)
		}
	} else if component {
		decoded, err = url.QueryUnescape(input)
		if err != nil {
			fmt.Fprintf(os.Stderr, "Error decoding query component: %v\n", err)
			os.Exit(1)
		}
	} else {
		decoded, err = url.PathUnescape(input)
		if err != nil {
			fmt.Fprintf(os.Stderr, "Error decoding path: %v\n", err)
			os.Exit(1)
		}
	}
	fmt.Println(decoded)
}
