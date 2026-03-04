<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes"/>
	<xsl:template match="docroot">
		<xsl:variable name="apos">'</xsl:variable>
		<!-- TODO Les configurations sont passées dans une balise dediee desormais -->
		<xsl:variable name="version">
			<xsl:value-of select="SiteConfiguration/@AppVersion"/>
		</xsl:variable>
		<xsl:variable name="dateGeneration">
			<xsl:value-of select="SiteConfiguration/@DateGeneration"/>
		</xsl:variable>
		
		<xsl:value-of select="concat('document.write(',$apos,'v',$version,' - Dernière actualisation: ', $dateGeneration,$apos,');')"/>
	</xsl:template>
</xsl:stylesheet>