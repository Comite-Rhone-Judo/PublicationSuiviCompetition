<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<!-- Un bandeau repliable d'epreuve. On suppose l'existence de liste des epreuve au niveau du '.' et qu'un template sera applique a chaque epreuve trouvee -->
	<xsl:template name="panelEpreuve">
		<xsl:param name="sexeCode"/>
		<xsl:param name="prefixPanel"/>
		<xsl:param name="imgPath"/>

		<xsl:variable name="panelApos">'</xsl:variable>

		<xsl:if test="count(./epreuve[@sexe = $sexeCode]) > 0">
			<div class="w3-panel w3-cell w3-mobile">
				<!-- La carte des donnees elle meme -->
				<div class="w3-card">
					<!-- entete avec un bouton permet d'ouvrir fermer le contenu de la carte -->
					<header class="w3-bar w3-light-green w3-large">
						<button class="w3-bar-item w3-light-green">
							<xsl:attribute name="onclick">
								<xsl:value-of select="concat('togglePanel(',$panelApos,$prefixPanel,$sexeCode,$panelApos,')')"/>
							</xsl:attribute>
							<img class="img" width="25" style="display: none;">
								<xsl:attribute name="src">
									<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
								</xsl:attribute>
								<xsl:attribute name="id">
									<xsl:value-of select="concat($prefixPanel,$sexeCode, 'Collapse')"/>
								</xsl:attribute>
							</img>
							<img class="img" width="25">
								<xsl:attribute name="src">
									<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
								</xsl:attribute>
								<xsl:attribute name="id">
									<xsl:value-of select="concat($prefixPanel, $sexeCode, 'Expand')"/>
								</xsl:attribute>
							</img>
							<xsl:choose>
								<xsl:when test="$sexeCode = 'M'">
									Masculins
								</xsl:when>
								<xsl:when test="$sexeCode = 'F'">
									Féminines
								</xsl:when>
								<xsl:when test="$sexeCode = 'X'">
									Mixte
								</xsl:when>
								<xsl:otherwise>
									Sans Catégorie
								</xsl:otherwise>
							</xsl:choose>
						</button>
					</header>
					<div class="w3-container" style="display:none;">
						<xsl:attribute name="id">
							<xsl:value-of select="concat($prefixPanel, $sexeCode)"/>
						</xsl:attribute>
						<xsl:apply-templates select="./epreuve[@sexe = $sexeCode]"/>
					</div>
				</div>
			</div>
		</xsl:if>

	</xsl:template>
	
</xsl:stylesheet>
