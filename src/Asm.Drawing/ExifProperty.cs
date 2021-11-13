using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asm.Drawing
{
    /// <summary>
    /// EXIF metadata property names.
    /// </summary>
    public enum ExifProperty
    {
        /// <summary>
        /// PropertyTagGpsVer
        /// </summary>
        PropertyTagGpsVer = 0x0000,
        /// <summary>
        /// PropertyTagGpsLatitudeRef
        /// </summary>
        PropertyTagGpsLatitudeRef = 0x0001,
        /// <summary>
        /// PropertyTagGpsLatitude
        /// </summary>
        PropertyTagGpsLatitude = 0x0002,
        /// <summary>
        /// PropertyTagGpsLongitudeRef
        /// </summary>
        PropertyTagGpsLongitudeRef = 0x0003,
        /// <summary>
        /// PropertyTagGpsLongitude
        /// </summary>
        PropertyTagGpsLongitude = 0x0004,
        /// <summary>
        /// PropertyTagGpsAltitudeRef
        /// </summary>
        PropertyTagGpsAltitudeRef = 0x0005,
        /// <summary>
        /// PropertyTagGpsAltitude
        /// </summary>
        PropertyTagGpsAltitude = 0x0006,
        /// <summary>
        /// PropertyTagGpsGpsTime
        /// </summary>
        PropertyTagGpsGpsTime = 0x0007,
        /// <summary>
        /// PropertyTagGpsGpsSatellites
        /// </summary>
        PropertyTagGpsGpsSatellites = 0x0008,
        /// <summary>
        /// PropertyTagGpsGpsStatus
        /// </summary>
        PropertyTagGpsGpsStatus = 0x0009,
        /// <summary>
        /// PropertyTagGpsGpsMeasureMode
        /// </summary>
        PropertyTagGpsGpsMeasureMode = 0x000A,
        /// <summary>
        /// PropertyTagGpsGpsDop
        /// </summary>
        PropertyTagGpsGpsDop = 0x000B,
        /// <summary>
        /// PropertyTagGpsSpeedRef
        /// </summary>
        PropertyTagGpsSpeedRef = 0x000C,
        /// <summary>
        /// PropertyTagGpsSpeed
        /// </summary>
        PropertyTagGpsSpeed = 0x000D,
        /// <summary>
        /// PropertyTagGpsTrackRef
        /// </summary>
        PropertyTagGpsTrackRef = 0x000E,
        /// <summary>
        /// PropertyTagGpsTrack
        /// </summary>
        PropertyTagGpsTrack = 0x000F,
        /// <summary>
        /// PropertyTagGpsImgDirRef
        /// </summary>
        PropertyTagGpsImgDirRef = 0x0010,
        /// <summary>
        /// PropertyTagGpsImgDir
        /// </summary>
        PropertyTagGpsImgDir = 0x0011,
        /// <summary>
        /// PropertyTagGpsMapDatum
        /// </summary>
        PropertyTagGpsMapDatum = 0x0012,
        /// <summary>
        /// PropertyTagGpsDestLatRef
        /// </summary>
        PropertyTagGpsDestLatRef = 0x0013,
        /// <summary>
        /// PropertyTagGpsDestLat
        /// </summary>
        PropertyTagGpsDestLat = 0x0014,
        /// <summary>
        /// PropertyTagGpsDestLongRef
        /// </summary>
        PropertyTagGpsDestLongRef = 0x0015,
        /// <summary>
        /// PropertyTagGpsDestLong
        /// </summary>
        PropertyTagGpsDestLong = 0x0016,
        /// <summary>
        /// PropertyTagGpsDestBearRef
        /// </summary>
        PropertyTagGpsDestBearRef = 0x0017,
        /// <summary>
        /// PropertyTagGpsDestBear
        /// </summary>
        PropertyTagGpsDestBear = 0x0018,
        /// <summary>
        /// PropertyTagGpsDestDistRef
        /// </summary>
        PropertyTagGpsDestDistRef = 0x0019,
        /// <summary>
        /// PropertyTagGpsDestDist
        /// </summary>
        PropertyTagGpsDestDist = 0x001A,
        /// <summary>
        /// PropertyTagNewSubfileType
        /// </summary>
        PropertyTagNewSubfileType = 0x00FE,
        /// <summary>
        /// PropertyTagSubfileType
        /// </summary>
        PropertyTagSubfileType = 0x00FF,
        /// <summary>
        /// PropertyTagImageWidth
        /// </summary>
        PropertyTagImageWidth = 0x0100,
        /// <summary>
        /// PropertyTagImageHeight
        /// </summary>
        PropertyTagImageHeight = 0x0101,
        /// <summary>
        /// PropertyTagBitsPerSample
        /// </summary>
        PropertyTagBitsPerSample = 0x0102,
        /// <summary>
        /// PropertyTagCompression
        /// </summary>
        PropertyTagCompression = 0x0103,
        /// <summary>
        /// PropertyTagPhotometricInterp
        /// </summary>
        PropertyTagPhotometricInterp = 0x0106,
        /// <summary>
        /// PropertyTagThreshHolding
        /// </summary>
        PropertyTagThreshHolding = 0x0107,
        /// <summary>
        /// PropertyTagCellWidth
        /// </summary>
        PropertyTagCellWidth = 0x0108,
        /// <summary>
        /// PropertyTagCellHeight
        /// </summary>
        PropertyTagCellHeight = 0x0109,
        /// <summary>
        /// PropertyTagFillOrder
        /// </summary>
        PropertyTagFillOrder = 0x010A,
        /// <summary>
        /// PropertyTagDocumentName
        /// </summary>
        PropertyTagDocumentName = 0x010D,
        /// <summary>
        /// PropertyTagImageDescription
        /// </summary>
        PropertyTagImageDescription = 0x010E,
        /// <summary>
        /// PropertyTagEquipMake
        /// </summary>
        PropertyTagEquipMake = 0x010F,
        /// <summary>
        /// PropertyTagEquipModel
        /// </summary>
        PropertyTagEquipModel = 0x0110,
        /// <summary>
        /// PropertyTagStripOffsets
        /// </summary>
        PropertyTagStripOffsets = 0x0111,
        /// <summary>
        /// PropertyTagOrientation
        /// </summary>
        PropertyTagOrientation = 0x0112,
        /// <summary>
        /// PropertyTagSamplesPerPixel
        /// </summary>
        PropertyTagSamplesPerPixel = 0x0115,
        /// <summary>
        /// PropertyTagRowsPerStrip
        /// </summary>
        PropertyTagRowsPerStrip = 0x0116,
        /// <summary>
        /// PropertyTagStripBytesCount
        /// </summary>
        PropertyTagStripBytesCount = 0x0117,
        /// <summary>
        /// PropertyTagMinSampleValue
        /// </summary>
        PropertyTagMinSampleValue = 0x0118,
        /// <summary>
        /// PropertyTagMaxSampleValue
        /// </summary>
        PropertyTagMaxSampleValue = 0x0119,
        /// <summary>
        /// PropertyTagXResolution
        /// </summary>
        PropertyTagXResolution = 0x011A,
        /// <summary>
        /// PropertyTagYResolution
        /// </summary>
        PropertyTagYResolution = 0x011B,
        /// <summary>
        /// PropertyTagPlanarConfig
        /// </summary>
        PropertyTagPlanarConfig = 0x011C,
        /// <summary>
        /// PropertyTagPageName
        /// </summary>
        PropertyTagPageName = 0x011D,
        /// <summary>
        /// PropertyTagXPosition
        /// </summary>
        PropertyTagXPosition = 0x011E,
        /// <summary>
        /// PropertyTagYPosition
        /// </summary>
        PropertyTagYPosition = 0x011F,
        /// <summary>
        /// PropertyTagFreeOffset
        /// </summary>
        PropertyTagFreeOffset = 0x0120,
        /// <summary>
        /// PropertyTagFreeByteCounts
        /// </summary>
        PropertyTagFreeByteCounts = 0x0121,
        /// <summary>
        /// PropertyTagGrayResponseUnit
        /// </summary>
        PropertyTagGrayResponseUnit = 0x0122,
        /// <summary>
        /// PropertyTagGrayResponseCurve
        /// </summary>
        PropertyTagGrayResponseCurve = 0x0123,
        /// <summary>
        /// PropertyTagT4Option
        /// </summary>
        PropertyTagT4Option = 0x0124,
        /// <summary>
        /// PropertyTagT6Option
        /// </summary>
        PropertyTagT6Option = 0x0125,
        /// <summary>
        /// PropertyTagResolutionUnit
        /// </summary>
        PropertyTagResolutionUnit = 0x0128,
        /// <summary>
        /// PropertyTagPageNumber
        /// </summary>
        PropertyTagPageNumber = 0x0129,
        /// <summary>
        /// PropertyTagTransferFunction
        /// </summary>
        PropertyTagTransferFunction = 0x012D,
        /// <summary>
        /// PropertyTagSoftwareUsed
        /// </summary>
        PropertyTagSoftwareUsed = 0x0131,
        /// <summary>
        /// PropertyTagDateTime
        /// </summary>
        PropertyTagDateTime = 0x0132,
        /// <summary>
        /// PropertyTagArtist
        /// </summary>
        PropertyTagArtist = 0x013B,
        /// <summary>
        /// PropertyTagHostComputer
        /// </summary>
        PropertyTagHostComputer = 0x013C,
        /// <summary>
        /// PropertyTagPredictor
        /// </summary>
        PropertyTagPredictor = 0x013D,
        /// <summary>
        /// PropertyTagWhitePoint
        /// </summary>
        PropertyTagWhitePoint = 0x013E,
        /// <summary>
        /// PropertyTagPrimaryChromaticities
        /// </summary>
        PropertyTagPrimaryChromaticities = 0x013F,
        /// <summary>
        /// PropertyTagColorMap
        /// </summary>
        PropertyTagColorMap = 0x0140,
        /// <summary>
        /// PropertyTagHalftoneHints
        /// </summary>
        PropertyTagHalftoneHints = 0x0141,
        /// <summary>
        /// PropertyTagTileWidth
        /// </summary>
        PropertyTagTileWidth = 0x0142,
        /// <summary>
        /// PropertyTagTileLength
        /// </summary>
        PropertyTagTileLength = 0x0143,
        /// <summary>
        /// PropertyTagTileOffset
        /// </summary>
        PropertyTagTileOffset = 0x0144,
        /// <summary>
        /// PropertyTagTileByteCounts
        /// </summary>
        PropertyTagTileByteCounts = 0x0145,
        /// <summary>
        /// PropertyTagInkSet
        /// </summary>
        PropertyTagInkSet = 0x014C,
        /// <summary>
        /// PropertyTagInkNames
        /// </summary>
        PropertyTagInkNames = 0x014D,
        /// <summary>
        /// PropertyTagNumberOfInks
        /// </summary>
        PropertyTagNumberOfInks = 0x014E,
        /// <summary>
        /// PropertyTagDotRange
        /// </summary>
        PropertyTagDotRange = 0x0150,
        /// <summary>
        /// PropertyTagTargetPrinter
        /// </summary>
        PropertyTagTargetPrinter = 0x0151,
        /// <summary>
        /// PropertyTagExtraSamples
        /// </summary>
        PropertyTagExtraSamples = 0x0152,
        /// <summary>
        /// PropertyTagSampleFormat
        /// </summary>
        PropertyTagSampleFormat = 0x0153,
        /// <summary>
        /// PropertyTagSMinSampleValue
        /// </summary>
        PropertyTagSMinSampleValue = 0x0154,
        /// <summary>
        /// PropertyTagSMaxSampleValue
        /// </summary>
        PropertyTagSMaxSampleValue = 0x0155,
        /// <summary>
        /// PropertyTagTransferRange
        /// </summary>
        PropertyTagTransferRange = 0x0156,
        /// <summary>
        /// PropertyTagJPEGProc
        /// </summary>
        PropertyTagJPEGProc = 0x0200,
        /// <summary>
        /// PropertyTagJPEGInterFormat
        /// </summary>
        PropertyTagJPEGInterFormat = 0x0201,
        /// <summary>
        /// PropertyTagJPEGInterLength
        /// </summary>
        PropertyTagJPEGInterLength = 0x0202,
        /// <summary>
        /// PropertyTagJPEGRestartInterval
        /// </summary>
        PropertyTagJPEGRestartInterval = 0x0203,
        /// <summary>
        /// PropertyTagJPEGLosslessPredictors
        /// </summary>
        PropertyTagJPEGLosslessPredictors = 0x0205,
        /// <summary>
        /// PropertyTagJPEGPointTransforms
        /// </summary>
        PropertyTagJPEGPointTransforms = 0x0206,
        /// <summary>
        /// PropertyTagJPEGQTables
        /// </summary>
        PropertyTagJPEGQTables = 0x0207,
        /// <summary>
        /// PropertyTagJPEGDCTables
        /// </summary>
        PropertyTagJPEGDCTables = 0x0208,
        /// <summary>
        /// PropertyTagJPEGACTables
        /// </summary>
        PropertyTagJPEGACTables = 0x0209,
        /// <summary>
        /// PropertyTagYCbCrCoefficients
        /// </summary>
        PropertyTagYCbCrCoefficients = 0x0211,
        /// <summary>
        /// PropertyTagYCbCrSubsampling
        /// </summary>
        PropertyTagYCbCrSubsampling = 0x0212,
        /// <summary>
        /// PropertyTagYCbCrPositioning
        /// </summary>
        PropertyTagYCbCrPositioning = 0x0213,
        /// <summary>
        /// PropertyTagREFBlackWhite
        /// </summary>
        PropertyTagREFBlackWhite = 0x0214,
        /// <summary>
        /// PropertyTagGamma
        /// </summary>
        PropertyTagGamma = 0x0301,
        /// <summary>
        /// PropertyTagICCProfileDescriptor
        /// </summary>
        PropertyTagICCProfileDescriptor = 0x0302,
        /// <summary>
        /// PropertyTagSRGBRenderingIntent
        /// </summary>
        PropertyTagSRGBRenderingIntent = 0x0303,
        /// <summary>
        /// PropertyTagImageTitle
        /// </summary>
        PropertyTagImageTitle = 0x0320,
        /// <summary>
        /// PropertyTagResolutionXUnit
        /// </summary>
        PropertyTagResolutionXUnit = 0x5001,
        /// <summary>
        /// PropertyTagResolutionYUnit
        /// </summary>
        PropertyTagResolutionYUnit = 0x5002,
        /// <summary>
        /// PropertyTagResolutionXLengthUnit
        /// </summary>
        PropertyTagResolutionXLengthUnit = 0x5003,
        /// <summary>
        /// PropertyTagResolutionYLengthUnit
        /// </summary>
        PropertyTagResolutionYLengthUnit = 0x5004,
        /// <summary>
        /// PropertyTagPrintFlags
        /// </summary>
        PropertyTagPrintFlags = 0x5005,
        /// <summary>
        /// PropertyTagPrintFlagsVersion
        /// </summary>
        PropertyTagPrintFlagsVersion = 0x5006,
        /// <summary>
        /// PropertyTagPrintFlagsCrop
        /// </summary>
        PropertyTagPrintFlagsCrop = 0x5007,
        /// <summary>
        /// PropertyTagPrintFlagsBleedWidth
        /// </summary>
        PropertyTagPrintFlagsBleedWidth = 0x5008,
        /// <summary>
        /// PropertyTagPrintFlagsBleedWidthScale
        /// </summary>
        PropertyTagPrintFlagsBleedWidthScale = 0x5009,
        /// <summary>
        /// PropertyTagHalftoneLPI
        /// </summary>
        PropertyTagHalftoneLPI = 0x500A,
        /// <summary>
        /// PropertyTagHalftoneLPIUnit
        /// </summary>
        PropertyTagHalftoneLPIUnit = 0x500B,
        /// <summary>
        /// PropertyTagHalftoneDegree
        /// </summary>
        PropertyTagHalftoneDegree = 0x500C,
        /// <summary>
        /// PropertyTagHalftoneShape
        /// </summary>
        PropertyTagHalftoneShape = 0x500D,
        /// <summary>
        /// PropertyTagHalftoneMisc
        /// </summary>
        PropertyTagHalftoneMisc = 0x500E,
        /// <summary>
        /// PropertyTagHalftoneScreen
        /// </summary>
        PropertyTagHalftoneScreen = 0x500F,
        /// <summary>
        /// PropertyTagJPEGQuality
        /// </summary>
        PropertyTagJPEGQuality = 0x5010,
        /// <summary>
        /// PropertyTagGridSize
        /// </summary>
        PropertyTagGridSize = 0x5011,
        /// <summary>
        /// PropertyTagThumbnailFormat
        /// </summary>
        PropertyTagThumbnailFormat = 0x5012,
        /// <summary>
        /// PropertyTagThumbnailWidth
        /// </summary>
        PropertyTagThumbnailWidth = 0x5013,
        /// <summary>
        /// PropertyTagThumbnailHeight
        /// </summary>
        PropertyTagThumbnailHeight = 0x5014,
        /// <summary>
        /// PropertyTagThumbnailColorDepth
        /// </summary>
        PropertyTagThumbnailColorDepth = 0x5015,
        /// <summary>
        /// PropertyTagThumbnailPlanes
        /// </summary>
        PropertyTagThumbnailPlanes = 0x5016,
        /// <summary>
        /// PropertyTagThumbnailRawBytes
        /// </summary>
        PropertyTagThumbnailRawBytes = 0x5017,
        /// <summary>
        /// PropertyTagThumbnailSize
        /// </summary>
        PropertyTagThumbnailSize = 0x5018,
        /// <summary>
        /// PropertyTagThumbnailCompressedSize
        /// </summary>
        PropertyTagThumbnailCompressedSize = 0x5019,
        /// <summary>
        /// PropertyTagColorTransferFunction
        /// </summary>
        PropertyTagColorTransferFunction = 0x501A,
        /// <summary>
        /// PropertyTagThumbnailData
        /// </summary>
        PropertyTagThumbnailData = 0x501B,
        /// <summary>
        /// PropertyTagThumbnailImageWidth
        /// </summary>
        PropertyTagThumbnailImageWidth = 0x5020,
        /// <summary>
        /// PropertyTagThumbnailImageHeight
        /// </summary>
        PropertyTagThumbnailImageHeight = 0x5021,
        /// <summary>
        /// PropertyTagThumbnailBitsPerSample
        /// </summary>
        PropertyTagThumbnailBitsPerSample = 0x5022,
        /// <summary>
        /// PropertyTagThumbnailCompression
        /// </summary>
        PropertyTagThumbnailCompression = 0x5023,
        /// <summary>
        /// PropertyTagThumbnailPhotometricInterp
        /// </summary>
        PropertyTagThumbnailPhotometricInterp = 0x5024,
        /// <summary>
        /// PropertyTagThumbnailImageDescription
        /// </summary>
        PropertyTagThumbnailImageDescription = 0x5025,
        /// <summary>
        /// PropertyTagThumbnailEquipMake
        /// </summary>
        PropertyTagThumbnailEquipMake = 0x5026,
        /// <summary>
        /// PropertyTagThumbnailEquipModel
        /// </summary>
        PropertyTagThumbnailEquipModel = 0x5027,
        /// <summary>
        /// PropertyTagThumbnailStripOffsets
        /// </summary>
        PropertyTagThumbnailStripOffsets = 0x5028,
        /// <summary>
        /// PropertyTagThumbnailOrientation
        /// </summary>
        PropertyTagThumbnailOrientation = 0x5029,
        /// <summary>
        /// PropertyTagThumbnailSamplesPerPixel
        /// </summary>
        PropertyTagThumbnailSamplesPerPixel = 0x502A,
        /// <summary>
        /// PropertyTagThumbnailRowsPerStrip
        /// </summary>
        PropertyTagThumbnailRowsPerStrip = 0x502B,
        /// <summary>
        /// PropertyTagThumbnailStripBytesCount
        /// </summary>
        PropertyTagThumbnailStripBytesCount = 0x502C,
        /// <summary>
        /// PropertyTagThumbnailResolutionX
        /// </summary>
        PropertyTagThumbnailResolutionX = 0x502D,
        /// <summary>
        /// PropertyTagThumbnailResolutionY
        /// </summary>
        PropertyTagThumbnailResolutionY = 0x502E,
        /// <summary>
        /// PropertyTagThumbnailPlanarConfig
        /// </summary>
        PropertyTagThumbnailPlanarConfig = 0x502F,
        /// <summary>
        /// PropertyTagThumbnailResolutionUnit
        /// </summary>
        PropertyTagThumbnailResolutionUnit = 0x5030,
        /// <summary>
        /// PropertyTagThumbnailTransferFunction
        /// </summary>
        PropertyTagThumbnailTransferFunction = 0x5031,
        /// <summary>
        /// PropertyTagThumbnailSoftwareUsed
        /// </summary>
        PropertyTagThumbnailSoftwareUsed = 0x5032,
        /// <summary>
        /// PropertyTagThumbnailDateTime
        /// </summary>
        PropertyTagThumbnailDateTime = 0x5033,
        /// <summary>
        /// PropertyTagThumbnailArtist
        /// </summary>
        PropertyTagThumbnailArtist = 0x5034,
        /// <summary>
        /// PropertyTagThumbnailWhitePoint
        /// </summary>
        PropertyTagThumbnailWhitePoint = 0x5035,
        /// <summary>
        /// PropertyTagThumbnailPrimaryChromaticities
        /// </summary>
        PropertyTagThumbnailPrimaryChromaticities = 0x5036,
        /// <summary>
        /// PropertyTagThumbnailYCbCrCoefficients
        /// </summary>
        PropertyTagThumbnailYCbCrCoefficients = 0x5037,
        /// <summary>
        /// PropertyTagThumbnailYCbCrSubsampling
        /// </summary>
        PropertyTagThumbnailYCbCrSubsampling = 0x5038,
        /// <summary>
        /// PropertyTagThumbnailYCbCrPositioning
        /// </summary>
        PropertyTagThumbnailYCbCrPositioning = 0x5039,
        /// <summary>
        /// PropertyTagThumbnailRefBlackWhite
        /// </summary>
        PropertyTagThumbnailRefBlackWhite = 0x503A,
        /// <summary>
        /// PropertyTagThumbnailCopyRight
        /// </summary>
        PropertyTagThumbnailCopyRight = 0x503B,
        /// <summary>
        /// PropertyTagLuminanceTable
        /// </summary>
        PropertyTagLuminanceTable = 0x5090,
        /// <summary>
        /// PropertyTagChrominanceTable
        /// </summary>
        PropertyTagChrominanceTable = 0x5091,
        /// <summary>
        /// PropertyTagFrameDelay
        /// </summary>
        PropertyTagFrameDelay = 0x5100,
        /// <summary>
        /// PropertyTagLoopCount
        /// </summary>
        PropertyTagLoopCount = 0x5101,
        /// <summary>
        /// PropertyTagGlobalPalette
        /// </summary>
        PropertyTagGlobalPalette = 0x5102,
        /// <summary>
        /// PropertyTagIndexBackground
        /// </summary>
        PropertyTagIndexBackground = 0x5103,
        /// <summary>
        /// PropertyTagIndexTransparent
        /// </summary>
        PropertyTagIndexTransparent = 0x5104,
        /// <summary>
        /// PropertyTagPixelUnit
        /// </summary>
        PropertyTagPixelUnit = 0x5110,
        /// <summary>
        /// PropertyTagPixelPerUnitX
        /// </summary>
        PropertyTagPixelPerUnitX = 0x5111,
        /// <summary>
        /// PropertyTagPixelPerUnitY
        /// </summary>
        PropertyTagPixelPerUnitY = 0x5112,
        /// <summary>
        /// PropertyTagPaletteHistogram
        /// </summary>
        PropertyTagPaletteHistogram = 0x5113,
        /// <summary>
        /// PropertyTagCopyright
        /// </summary>
        PropertyTagCopyright = 0x8298,
        /// <summary>
        /// PropertyTagExifExposureTime
        /// </summary>
        PropertyTagExifExposureTime = 0x829A,
        /// <summary>
        /// PropertyTagExifFNumber
        /// </summary>
        PropertyTagExifFNumber = 0x829D,
        /// <summary>
        /// PropertyTagExifIFD
        /// </summary>
        PropertyTagExifIFD = 0x8769,
        /// <summary>
        /// PropertyTagICCProfile
        /// </summary>
        PropertyTagICCProfile = 0x8773,
        /// <summary>
        /// PropertyTagExifExposureProg
        /// </summary>
        PropertyTagExifExposureProg = 0x8822,
        /// <summary>
        /// PropertyTagExifSpectralSense
        /// </summary>
        PropertyTagExifSpectralSense = 0x8824,
        /// <summary>
        /// PropertyTagGpsIFD
        /// </summary>
        PropertyTagGpsIFD = 0x8825,
        /// <summary>
        /// PropertyTagExifISOSpeed
        /// </summary>
        PropertyTagExifISOSpeed = 0x8827,
        /// <summary>
        /// PropertyTagExifOECF
        /// </summary>
        PropertyTagExifOECF = 0x8828,
        /// <summary>
        /// PropertyTagExifVer
        /// </summary>
        PropertyTagExifVer = 0x9000,
        /// <summary>
        /// PropertyTagExifDTOrig
        /// </summary>
        PropertyTagExifDTOrig = 0x9003,
        /// <summary>
        /// PropertyTagExifDTDigitized
        /// </summary>
        PropertyTagExifDTDigitized = 0x9004,
        /// <summary>
        /// PropertyTagExifCompConfig
        /// </summary>
        PropertyTagExifCompConfig = 0x9101,
        /// <summary>
        /// PropertyTagExifCompBPP
        /// </summary>
        PropertyTagExifCompBPP = 0x9102,
        /// <summary>
        /// PropertyTagExifShutterSpeed
        /// </summary>
        PropertyTagExifShutterSpeed = 0x9201,
        /// <summary>
        /// PropertyTagExifAperture
        /// </summary>
        PropertyTagExifAperture = 0x9202,
        /// <summary>
        /// PropertyTagExifBrightness
        /// </summary>
        PropertyTagExifBrightness = 0x9203,
        /// <summary>
        /// PropertyTagExifExposureBias
        /// </summary>
        PropertyTagExifExposureBias = 0x9204,
        /// <summary>
        /// PropertyTagExifMaxAperture
        /// </summary>
        PropertyTagExifMaxAperture = 0x9205,
        /// <summary>
        /// PropertyTagExifSubjectDist
        /// </summary>
        PropertyTagExifSubjectDist = 0x9206,
        /// <summary>
        /// PropertyTagExifMeteringMode
        /// </summary>
        PropertyTagExifMeteringMode = 0x9207,
        /// <summary>
        /// PropertyTagExifLightSource
        /// </summary>
        PropertyTagExifLightSource = 0x9208,
        /// <summary>
        /// PropertyTagExifFlash
        /// </summary>
        PropertyTagExifFlash = 0x9209,
        /// <summary>
        /// PropertyTagExifFocalLength
        /// </summary>
        PropertyTagExifFocalLength = 0x920A,
        /// <summary>
        /// PropertyTagExifMakerNote
        /// </summary>
        PropertyTagExifMakerNote = 0x927C,
        /// <summary>
        /// PropertyTagExifUserComment
        /// </summary>
        PropertyTagExifUserComment = 0x9286,
        /// <summary>
        /// PropertyTagExifDTSubsec
        /// </summary>
        PropertyTagExifDTSubsec = 0x9290,
        /// <summary>
        /// PropertyTagExifDTOrigSS
        /// </summary>
        PropertyTagExifDTOrigSS = 0x9291,
        /// <summary>
        /// PropertyTagExifDTDigSS
        /// </summary>
        PropertyTagExifDTDigSS = 0x9292,
        /// <summary>
        /// PropertyTagExifFPXVer
        /// </summary>
        PropertyTagExifFPXVer = 0xA000,
        /// <summary>
        /// PropertyTagExifColorSpace
        /// </summary>
        PropertyTagExifColorSpace = 0xA001,
        /// <summary>
        /// PropertyTagExifPixXDim
        /// </summary>
        PropertyTagExifPixXDim = 0xA002,
        /// <summary>
        /// PropertyTagExifPixYDim
        /// </summary>
        PropertyTagExifPixYDim = 0xA003,
        /// <summary>
        /// PropertyTagExifRelatedWav
        /// </summary>
        PropertyTagExifRelatedWav = 0xA004,
        /// <summary>
        /// PropertyTagExifInterop
        /// </summary>
        PropertyTagExifInterop = 0xA005,
        /// <summary>
        /// PropertyTagExifFlashEnergy
        /// </summary>
        PropertyTagExifFlashEnergy = 0xA20B,
        /// <summary>
        /// PropertyTagExifSpatialFR
        /// </summary>
        PropertyTagExifSpatialFR = 0xA20C,
        /// <summary>
        /// PropertyTagExifFocalXRes
        /// </summary>
        PropertyTagExifFocalXRes = 0xA20E,
        /// <summary>
        /// PropertyTagExifFocalYRes
        /// </summary>
        PropertyTagExifFocalYRes = 0xA20F,
        /// <summary>
        /// PropertyTagExifFocalResUnit
        /// </summary>
        PropertyTagExifFocalResUnit = 0xA210,
        /// <summary>
        /// PropertyTagExifSubjectLoc
        /// </summary>
        PropertyTagExifSubjectLoc = 0xA214,
        /// <summary>
        /// PropertyTagExifExposureIndex
        /// </summary>
        PropertyTagExifExposureIndex = 0xA215,
        /// <summary>
        /// PropertyTagExifSensingMethod
        /// </summary>
        PropertyTagExifSensingMethod = 0xA217,
        /// <summary>
        /// PropertyTagExifFileSource
        /// </summary>
        PropertyTagExifFileSource = 0xA300,
        /// <summary>
        /// PropertyTagExifSceneType
        /// </summary>
        PropertyTagExifSceneType = 0xA301,
        /// <summary>
        /// PropertyTagExifCfaPattern
        /// </summary>
        PropertyTagExifCfaPattern = 0xA302,
    }
}
