package snowflakeid

import (
	"govobs/app/core/types"

	"github.com/bwmarrin/snowflake"
)

func NewIDCreator(node *snowflake.Node) types.IDCreator {
	return func() types.ID {
		return types.ID(node.Generate().Int64())
	}
}
