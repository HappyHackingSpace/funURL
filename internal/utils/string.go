package utils

import "strings"

func StringOrDefault(value string) string {
	if trimmed := strings.TrimSpace(value); trimmed != "" {
		return trimmed
	}
	return "N/A"
}
