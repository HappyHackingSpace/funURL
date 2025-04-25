package cli

import (
	"os"
 	"github.com/spf13/cobra"
	"fmt"
	"github.com/fatih/color"
 )

 
var rootCmd = &cobra.Command{
	Use:   "funURL",
	Short: "A Swiss Army Knife for URLs",
	Long: GetBanner() + `
funURL is a command-line tool designed as a Swiss Army knife for URL. 
It takes a URL as input (via stdin, command-line flags, or arguments) and provides 
a range of operations for parsing, validation, modification, encoding/decoding, 
and handling URLs efficiently.`,
}

func GetBanner() string {
	green := color.New(color.FgGreen).SprintFunc()
	reset := color.New(color.Reset).SprintFunc()
	red := color.New(color.FgRed).SprintFunc()
	fmt.Println(reset(`
   __           _    _ ___ _    
  / _|_  _ _ _ | |  | | _ \ |   
 |  _| || | ' \| |__| |   / |__ 
 |_|  \_,_|_||_|____|_|_|_\____|`))
	fmt.Printf(" %s <3 %s \n", green("A Swiss Army Knife for URLs"), red("@HappyHackingSpace"))
	fmt.Println(reset(""))	
	return ""
}

func Execute() {
	err := rootCmd.Execute()
	if err != nil {
		os.Exit(1)
	}
}
