<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<!-- ===================================================================== -->
	<!-- COMPOSANT GÉNÉRIQUE : Bouton Accordéon (Réutilisable partout)         -->
	<!-- ===================================================================== -->
	<xsl:template name="AccordionButton">
		<xsl:param name="sexeCode"/>
		<xsl:param name="targetId"/>
		<xsl:param name="SiteRoutes"/>
		
		<xsl:variable name="imgPath" select="$SiteRoutes/*/@urlImg"/>

		<!-- CORRECTION : Suppression de $apos et utilisation directe des apostrophes -->
		<button class="ios-accordion-btn" onclick="togglePanel('{$targetId}')">
			<span>
				<xsl:choose>
					<xsl:when test="$sexeCode = 'M'">Masculins</xsl:when>
					<xsl:when test="$sexeCode = 'F'">Féminines</xsl:when>
					<xsl:when test="$sexeCode = 'X'">Mixte</xsl:when>
					<xsl:otherwise>Sans Catégorie</xsl:otherwise>
				</xsl:choose>
			</span>
			<div>
				<img class="tas-accordion-icon tas-icon-hidden" src="{$imgPath}up_circular-32.png" id="{concat($targetId, 'Collapse')}"/>
				<img class="tas-accordion-icon tas-icon-visible" src="{$imgPath}down_circular-32.png" id="{concat($targetId, 'Expand')}"/>
			</div>
		</button>
	</xsl:template>

	<!-- ===================================================================== -->
	<!-- WRAPPER SPÉCIFIQUE : Panneau complet pour les Épreuves Simples        -->
	<!-- ===================================================================== -->
	<xsl:template name="panelEpreuve">
		<xsl:param name="sexeCode"/>
		<xsl:param name="prefixPanel"/>
		<xsl:param name="imgPath"/>

		<xsl:if test="count(./epreuve[@sexe = $sexeCode]) > 0">
			<div class="w3-col l4 m6 s12 w3-margin-bottom w3-padding-small">

				<!-- 1. Appel du composant visuel -->
				<xsl:call-template name="AccordionButton">
					<xsl:with-param name="sexeCode" select="$sexeCode"/>
					<xsl:with-param name="targetId" select="concat($prefixPanel, $sexeCode)"/>
					<xsl:with-param name="imgPath" select="$imgPath"/>
				</xsl:call-template>

				<!-- 2. Injection de la logique spécifique (Liste d'épreuves) -->
				<div class="tasClosedPanelType tas-accordion-content-hidden w3-container" id="{concat($prefixPanel, $sexeCode)}">
					<xsl:apply-templates select="./epreuve[@sexe = $sexeCode]"/>
				</div>

			</div>
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>