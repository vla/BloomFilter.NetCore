using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("931DFF3C-9F5A-4551-B36C-2300844DDDC2")]


#if NETSTANDARD2_1 || NET6_0_OR_GREATER || NETSTANDARD2_0 || NET40
[assembly: AllowPartiallyTrustedCallers]
#endif
#if NETSTANDARD2_1 || NET6_0_OR_GREATER || NETSTANDARD2_0 || NET40 || NET45
[assembly: SecurityRules(SecurityRuleSet.Level1)]
#endif


[assembly: InternalsVisibleTo("BloomFilterTest, PublicKey=00240000048000009400000006020000002400005253413100040000010001004dd575590e774ea5ea5b02c96ec45feb8720721159f7d7a463680d1dcca3ddc5c5f52dd0e3f7086a6504df00571f9816545ac9ad6d402370bd6f9a395e08a9fe7ff26ce99318bab6a7fdc8233e9ce22c81d7fd75508d79bb7a93f6440656282bd7c95a3f891258075626eea022d03d0859954641424cbb15e031b6fec15541d3")]
[assembly: InternalsVisibleTo("PerformanceTest, PublicKey=00240000048000009400000006020000002400005253413100040000010001004dd575590e774ea5ea5b02c96ec45feb8720721159f7d7a463680d1dcca3ddc5c5f52dd0e3f7086a6504df00571f9816545ac9ad6d402370bd6f9a395e08a9fe7ff26ce99318bab6a7fdc8233e9ce22c81d7fd75508d79bb7a93f6440656282bd7c95a3f891258075626eea022d03d0859954641424cbb15e031b6fec15541d3")]
[assembly: InternalsVisibleTo("BenchmarkTest, PublicKey=00240000048000009400000006020000002400005253413100040000010001004dd575590e774ea5ea5b02c96ec45feb8720721159f7d7a463680d1dcca3ddc5c5f52dd0e3f7086a6504df00571f9816545ac9ad6d402370bd6f9a395e08a9fe7ff26ce99318bab6a7fdc8233e9ce22c81d7fd75508d79bb7a93f6440656282bd7c95a3f891258075626eea022d03d0859954641424cbb15e031b6fec15541d3")]