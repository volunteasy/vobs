using System.Data;
using IdGen;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Volunteasy.Core.Data;

public class IdValueGenerator : ValueGenerator<long>
{
    public override long Next(EntityEntry entry)
    {
        var snowflake = entry.Context.GetService<IdGenerator>();
        if (snowflake == null)
            throw new ConstraintException("could not get snowflake instance");
        
        return snowflake.CreateId();
    }

    public override bool GeneratesTemporaryValues => false;
}