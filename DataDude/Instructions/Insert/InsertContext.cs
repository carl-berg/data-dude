﻿using System.Collections.Generic;
using DataDude.Instructions.Insert.Insertion;
using DataDude.Instructions.Insert.Interception;
using DataDude.Instructions.Insert.ValueProviders;

namespace DataDude.Instructions.Insert
{
    public class InsertContext
    {
        public InsertContext(DataDudeContext context)
        {
            context.Set("InsertContext", this);
        }

        public IList<IInsertInterceptor> InsertInterceptors { get; } = new List<IInsertInterceptor>
        {
            new IdentityInsertInterceptor(),
        };

        public IList<IValueProvider> InsertValueProviders { get; } = new List<IValueProvider>
        {
            new StringValueProvider(),
            new NumericValueProvider(),
            new BinaryValueProvider(),
            new DateValueProvider(),
            new BoolValueProvider(),
            new VersionValueProvider(),
            new GuidValueProvider()
        };

        public IList<IInsertRowHandler> InsertRowHandlers { get; } = new List<IInsertRowHandler>
        {
            new IdentityInsertRowHandler(),
            new GeneratingInsertRowHandler(new UniqueValueGenerator()),
        };

        public IList<InsertedRow> InsertedRows { get; } = new List<InsertedRow>();

        public static InsertContext? Get(DataDudeContext context) => context.Get<InsertContext>("InsertContext");
    }
}
