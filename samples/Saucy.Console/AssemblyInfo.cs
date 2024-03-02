// <copyright file="AssemblyInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Saucy.Common.Attributes;
using Saucy.Common.Enums;

[assembly: SaucyInclude]
[assembly: SaucyAddNamespace(nameof(Saucy.Console.Builders), ServiceScope.Transient)]
[assembly: SaucyAddNamespace(nameof(Saucy.Console.Services), ServiceScope.Transient)]