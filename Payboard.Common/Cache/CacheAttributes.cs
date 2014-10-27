using System;

namespace Payboard.Common.Cache
{
    // See http://stackoverflow.com/a/4051391/68231

    /// <summary>
    /// Signify to Ninject that we want to load the ShortTermCache instance
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class ShortTermCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the StandardCache instance
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class StandardCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the a long-term cache, instead of a simple standard instance
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class LongTermCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the CustomerUserOutboxItemsStatsCache instance of the standard cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class CustomerUserOutboxItemStatsCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the PathDiscoveryContextCache instance of the standard cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class PathDiscoveryContextCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the PathAnalysisContextCache instance of the standard cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class PathAnalysisContextCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the PathNodeAnalysesCache instance of the standard cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class PathNodeAnalysesCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the SubseqentEventAnalysesCache instance of the standard cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class SubseqentEventAnalysesCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the SegmentedIdsCache instance instead of the standard cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class SegmentedIdsCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the CoalescedIds instance instead of the standard cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class CoalescedIdsCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the ProcessingSegmentCache instance instead of the standard cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class ProcessingSegmentCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the PathComparisonContextCache instance instead of the standard cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class PathComparisonContextCacheAttribute : Attribute
    {
    }

    /// <summary>
    /// Signify to Ninject that we want to load the PathComparisonContextCache instance instead of the standard cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public class AllPathsAnalysisContextCacheAttribute : Attribute
    {
    }

}