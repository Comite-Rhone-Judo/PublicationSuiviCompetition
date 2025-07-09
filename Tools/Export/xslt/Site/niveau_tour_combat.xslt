<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<!-- TEMPLATE NIVEAU TOUR COMBAT -->
	<xsl:template name="NiveauTourCombat">
		<xsl:param name="combat"/>
		<xsl:param name="typePhase"/>
		<!-- 1 = Poule, 2 = Tableau -->
		<xsl:param name="repechage"/>
		<!-- utilise true() ou false() -->


		<!-- Affiche les informations de niveau pour un tableau (typePhase = 2) -->
		<xsl:if test="$typePhase = 2">

			<!-- Calcul le niveau 8eme/16eme de la phase -->
			<xsl:variable name="niveauPhase">
				<xsl:call-template name="PowerOf2">
					<xsl:with-param name="power" select="$combat/@niveau - 1"/>
				</xsl:call-template>
			</xsl:variable>

			<xsl:choose>
				<xsl:when test="$repechage">
					<xsl:choose>
						<xsl:when test="starts-with($combat/@reference, '3') or starts-with($combat/@reference, '5') or starts-with($combat/@reference, '7')">
							<xsl:value-of select=" concat('Barrage ', substring($combat/@reference,1,1))"/>
						</xsl:when>
						<xsl:when test="$combat/@reference = '2.1.1' or $combat/@reference = '2.1.2'">Place de 3ème</xsl:when>
						<xsl:otherwise>Repêchage</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>
					<xsl:choose>
						<xsl:when test="$niveauPhase = 1">Finale</xsl:when>
						<xsl:when test="$niveauPhase = 2">Demi-finale</xsl:when>
						<xsl:when test="$niveauPhase = 4">Quart de finale</xsl:when>
						<xsl:when test="$niveauPhase > 0">
							<xsl:value-of select="$niveauPhase"/>
							<sup>ème</sup>
						</xsl:when>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>

		<!-- Affiche les informations de phase pour une poule (typePhase = 1) -->
		<xsl:if test="$typePhase = 1">
			<xsl:value-of select="concat('Poule ', $combat/@reference)"/>
		</xsl:if>

	</xsl:template>

	<!-- Utilitaire de calcul -->
	<xsl:template name="PowerOf2">
		<xsl:param name="power"/>

		<xsl:variable name="powerTMP">
			<xsl:choose>
				<xsl:when test="$power &lt; 0">
					<xsl:value-of select="$power * (-1)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$power"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="$power = 0">
				<xsl:value-of select="1"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="temp">
					<xsl:call-template name="PowerOf2">
						<xsl:with-param name="power" select="$powerTMP - 1"/>
					</xsl:call-template>
				</xsl:variable>
				<xsl:value-of select="2 * $temp"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>
