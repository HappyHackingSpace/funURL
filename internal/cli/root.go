package cli

import (
	"os"

	"github.com/spf13/cobra"
)

var rootCmd = &cobra.Command{
	Use:   "funURL",
	Short: "A Swiss Army Knife for URLs",
	Long: `funURL is a command-line tool designed as a Swiss Army knife for URL. 
It takes a URL as input (via stdin, command-line flags, or arguments) and provides 
a range of operations for parsing, validation, modification, encoding/decoding, 
and handling URLs efficiently.`,
}

func Execute() {
	err := rootCmd.Execute()
	if err != nil {
		os.Exit(1)
	}
}
