<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes"/>
	<xsl:template match="/">
		<xsl:variable name="apos">'</xsl:variable>

		<xsl:variable name="version">
			<xsl:value-of select="/competitions/competition[1]/@AppVersion"/>
		</xsl:variable>
		<xsl:variable name="dateGeneration">
			<xsl:value-of select="/competitions/competition[1]/@DateGeneration"/>
		</xsl:variable>
		
		<xsl:value-of select="concat('document.write(',$apos,'v',$version,' - Dernière actualisation: ', $dateGeneration,$apos,');')"/>
	</xsl:template>
</xsl:stylesheet>