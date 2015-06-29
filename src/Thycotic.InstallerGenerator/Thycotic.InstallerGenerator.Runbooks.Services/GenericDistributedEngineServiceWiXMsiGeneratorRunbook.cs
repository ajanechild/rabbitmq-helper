﻿using Thycotic.InstallerGenerator.Core.Steps;
using Thycotic.InstallerGenerator.Core.WiX;

namespace Thycotic.InstallerGenerator.Runbooks.Services
{
    /// <summary>
    /// Distributed engine service WiX MSI generator runbook
    /// </summary>
    public class GenericDistributedEngineServiceWiXMsiGeneratorRunbook : WiXMsiGeneratorRunbook
    {
        /// <summary>
        /// The default artifact name
        /// </summary>
        public override string DefaultArtifactName
        {
            get { return "Thycotic.DistributedEngine.Service"; }
        }

        /// <summary>
        /// Bakes the steps.
        /// </summary>
        /// <exception cref="System.ArgumentException">Engine to server communication ingredients missing.</exception>
        public override void BakeSteps()
        {
            Steps = new IInstallerGeneratorStep[]
            {
                new ExternalProcessStep
                {
                    Name = "File harvest (WiX Heat process)",
                    WorkingPath = WorkingPath,
                    ExecutablePath = HeatPathProvider(ApplicationPath),// ToolPaths.GetHeatPath(ApplicationPath),
                    Parameters = string.Format(@"
dir {0}
-nologo
-o output\Autogenerated.wxs 
-ag 
-sfrags 
-suid 
-cg main_component_group 
-t add_service_install.xsl 
-sreg 
-scom 
-srd 
-template fragment 
-dr INSTALLLOCATION", SourcePath)

                },
                new ExternalProcessStep
                {
                    Name = "Compiling (WiX Candle process)",
                    WorkingPath = WorkingPath,
                    ExecutablePath = CandlePathProvider(ApplicationPath),// ToolPaths.GetCandlePath(ApplicationPath),
                    Parameters = string.Format(@"
-fips
-nologo 
-arch x64
-ext WixUtilExtension 
-dInstallerVersion={0} 
-out output\
output\AutoGenerated.wxs Product.wxs", Version)
                },
                new ExternalProcessStep
                {
                    Name = "Linking and binding (WiX Light process)",
                    WorkingPath = WorkingPath,
                    ExecutablePath = LightPathProvider(ApplicationPath),// ToolPaths.GetLightPath(ApplicationPath),
                    Parameters = string.Format(@"
-nologo
-b {0}
-sval 
-ext WixUIExtension 
-ext WixUtilExtension 
-out {1}
output\AutoGenerated.wixobj output\Product.wixobj", SourcePath, ArtifactName)
                },
                new ExternalProcessStep
                {
                    Name = "Signing",
                    WorkingPath = WorkingPath,
                    ExecutablePath = SignToolPathProvider(ApplicationPath),
                    Parameters = string.Format(@"
sign 
/t http://timestamp.digicert.com 
/f {0}
/p {1}
{2}", PfxPath, PfxPassword, ArtifactName)
                }
            };
        }
    }
}
