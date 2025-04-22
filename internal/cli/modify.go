package cli

import (
	"HappyHackingSpace/funURL/internal/utils"
	"bufio"
	"fmt"
	"os"
	"strings"

	"github.com/spf13/cobra"
)

var (
	_protocol  string
	_path      string
	_query     string
	_fragments string
)

var modifyCmd = &cobra.Command{
	Use:   "modify",
	Short: "Add, remove, or update query parameters, change protocols, or modify paths",
	Run: func(cmd *cobra.Command, args []string) {
		if len(_protocol) == 0 && len(_path) == 0 && len(_query) == 0 && len(_fragments) == 0 {
			err := cmd.Help()
			if err != nil {
				fmt.Println("Error displaying help:", err)
			}
			return
		}

		urlString, _ := cmd.Flags().GetString("url")
		if urlString != "" {
			modifyUrl(urlString)
		} else if len(args) > 0 {
			modifyUrl(args[0])
		} else {
			scanner := bufio.NewScanner(os.Stdin)
			if scanner.Scan() {
				input := strings.TrimSpace(scanner.Text())
				modifyUrl(input)
			} else {
				fmt.Println("Error: No input provided")
				os.Exit(1)
			}
		}
	}}

func init() {
	rootCmd.AddCommand(modifyCmd)
	modifyCmd.Flags().StringP("url", "u", "", "URL to modify")
	modifyCmd.Flags().StringVarP(&_protocol, "protocol", "c", "", "modify scheme / protocol")
	modifyCmd.Flags().StringVarP(&_path, "path", "p", "", "modify path")
	modifyCmd.Flags().StringVarP(&_query, "query", "q", "", "modify query string")
	modifyCmd.Flags().StringVarP(&_fragments, "fragments", "f", "", "modify fragments")
}

func modifyUrl(urlString string) {
	modifiedUrl := utils.CreateURL(urlString)

	if len(_protocol) > 0 {
		modifiedUrl.Scheme = _protocol
	}

	if len(_path) > 0 {
		modifiedUrl.Path = _path
	}

	if len(_query) > 0 {
		modifiedUrl.RawQuery = _query
	}

	if len(_fragments) > 0 {
		modifiedUrl.Fragment = _fragments
	}

	fmt.Println(modifiedUrl)
}
