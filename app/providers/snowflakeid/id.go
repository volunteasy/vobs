package snowflakeid

import (
	"github.com/bwmarrin/snowflake"
	"govobs/app/core/types"
)

func NewIDCreator(node *snowflake.Node) types.IDCreator {
	return func() types.ID {
		return types.ID(node.Generate().Int64())
	}
}
