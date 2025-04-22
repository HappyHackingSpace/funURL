package utils

import (
	"net/url"
)

func CreateURL(str string) *url.URL {
	u, err := url.Parse(str)
	if err != nil {
		panic(err)
	}
	if u.Scheme == "" && u.Host == "" {
		panic("invalid url")
	}
	return u
}
