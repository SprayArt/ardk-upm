using System;
using System.Collections.Generic;

namespace Niantic.Lightship.AR
{
    public class AreaTargetsResult
    {
        /// <summary>
        /// The AreaTargetsResult class represents the response object returned after making a VPS coverage query from the
        /// CoverageClientManager. The VPS Coverage information is captured as a collection of AreaTarget structs.
        /// </summary>
        public AreaTargetsResult(
            LatLng queryLocation,
            int queryRadius,
            ResponseStatus status,
            CoverageAreasResult areasResult,
            LocalizationTargetsResult targetsResult,
            LocalizationTarget[] privateScanLocalizationTargets)
        {
            QueryLocation = queryLocation;
            QueryRadius = queryRadius;
            Status = status;

            var coverageAreas = areasResult?.Areas ?? Array.Empty<CoverageArea>();
            var localizationTargets = targetsResult?.ActivationTargets ?? new Dictionary<string, LocalizationTarget>();
            AreaTargets = new List<AreaTarget>();

            foreach (var area in coverageAreas)
            {
                foreach (string targetIdentifier in area.LocalizationTargetIdentifiers)
                {
                    if (localizationTargets.ContainsKey(targetIdentifier))
                    {
                        var areaTarget = new AreaTarget(area, localizationTargets[targetIdentifier]);
                        AreaTargets.Add(areaTarget);
                    }
                }
            }

            if (privateScanLocalizationTargets != null)
            {
                // Always show all private scans, regardless of distance.
                foreach (var privateScanLocalizationTarget in privateScanLocalizationTargets)
                {
                    // For now, link an generated coverage area to a private scan until the infrastructure is ready for
                    // a private scan to be tied to some coverage area
                    var privateScanCoverageArea = new CoverageArea(
                        new[] { privateScanLocalizationTarget.Identifier },
                        new[] { privateScanLocalizationTarget.Center },
                        CoverageArea.Localizability.EXPERIMENTAL.ToString());
                    var areaTarget = new AreaTarget(privateScanCoverageArea, privateScanLocalizationTarget);
                    AreaTargets.Add(areaTarget);
                }
            }
        }

        public LatLng QueryLocation { get; }
        public int QueryRadius { get; }
        public ResponseStatus Status { get; }
        public List<AreaTarget> AreaTargets { get; }
    }
}
