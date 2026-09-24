<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
	<!ENTITY nl "&#10;">
]>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<!-- TEMPLATE pour retourner le nom d'une structure (Club, etc.) en fonction du niveau de la competition -->
	<xsl:template name="LibelleStructure">
		<xsl:param name="ecartement" />
		<xsl:param name="typeCompetition" />
		<xsl:param name="club" />
		<xsl:param name="comite" />
		<xsl:param name="ligue" />
		<xsl:param name="pays" />

		<xsl:choose>
			<!-- Club = 2 => {Club} -->
			<xsl:when test="$ecartement = '2'">
				<xsl:value-of select="$club" />
			</xsl:when>

			<!-- Departement = 3 => {Club} - {Comite} -->
			<xsl:when test="$ecartement = '3'">
				<xsl:value-of select="$club" />&nbsp;-&nbsp;<xsl:value-of select="$comite" />
			</xsl:when>

			<!-- Ligue = 4 => {Club} - {Ligue} - {Comite} -->
			<xsl:when test="$ecartement = '4'">
				<xsl:value-of select="$club" />&nbsp;-&nbsp;<xsl:value-of select="$ligue" />&nbsp;-&nbsp;<xsl:value-of select="$comite" />
			</xsl:when>

			<!-- National = 5 => {Club} - {Pays} -->
			<xsl:when test="$ecartement = '5' or $ecartement = '6'">
				<xsl:value-of select="$club" />&nbsp;-&nbsp;<xsl:value-of select="$pays" />
			</xsl:when>

			<!-- Defaut => {Club} -->
			<xsl:otherwise>
				<xsl:value-of select="$club" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- TEMPLATE pour afficher l'intitule d'un groupe de structure -->
	<xsl:template name="LibelleGroupeStructure">
		<!-- Parametres obligatoires -->
		<xsl:param name="typeGroupe" />
		<xsl:param name="niveauCompetition" />
		<xsl:param name="entiteId" />
		<!-- Injection du referentiel pour etre autonome -->
		<xsl:param name="RefData" />
		<!-- Parametre optionnel pour le mode titre (ex: "Club XXX") -->
		<xsl:param name="avecPrefixe" select="'false'" />

		<xsl:choose>
			<!-- Type 1 = Alphabetique (Lettre) -->
			<xsl:when test="$typeGroupe = '1'">
				<xsl:if test="$avecPrefixe = 'true'">
					<xsl:text disable-output-escaping="yes">Nom commençant par</xsl:text>&nbsp;
				</xsl:if>
				<xsl:value-of select="$entiteId"/>
			</xsl:when>

			<!-- Type 2 = Club -->
			<xsl:when test="$typeGroupe = '2'">
				<xsl:if test="$avecPrefixe = 'true'">
					<xsl:text disable-output-escaping="yes">Club</xsl:text>&nbsp;
				</xsl:if>
				<xsl:variable name="club" select="$RefData/structures/clubs/club[@ID = $entiteId]" />
				<xsl:value-of select="$club/nom"/>

				<xsl:choose>
					<!-- Region (3) => {Club} ({Comite}) -->
					<xsl:when test="$niveauCompetition = '3'">
						<xsl:text disable-output-escaping="yes">&nbsp;(</xsl:text>
						<!-- Comité en complément = ID -->
						<xsl:value-of select="$club/@comite"/>
						<xsl:text>)</xsl:text>
					</xsl:when>

					<!-- National (4) => {Club} ({Ligue} - {Comite}) -->
					<xsl:when test="$niveauCompetition = '4'">
						<xsl:variable name="comite" select="$RefData/structures/comites/comite[@ID = $club/@comite]" />
						<xsl:text disable-output-escaping="yes">&nbsp;(</xsl:text>
						<xsl:value-of select="$RefData/structures/ligues/ligue[@ID = $comite/@ligue]/nom"/>
						<xsl:text disable-output-escaping="yes">&nbsp;-&nbsp;</xsl:text>
						<!-- Comité en complément = ID -->
						<xsl:value-of select="$comite/@ID"/>
						<xsl:text>)</xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>

			<!-- Type 3 = Comite -->
			<xsl:when test="$typeGroupe = '3'">
				<xsl:if test="$avecPrefixe = 'true'">
					<xsl:text disable-output-escaping="yes">Comité</xsl:text>&nbsp;
				</xsl:if>
				<xsl:variable name="comite" select="$RefData/structures/comites/comite[@ID = $entiteId]" />

				<!-- Le comité est l'entité principale = Nom complet -->
				<xsl:value-of select="$comite/nom"/>

				<xsl:if test="$niveauCompetition = '4'">
					<!-- National (4) => {Comite} ({Ligue}) -->
					<xsl:text disable-output-escaping="yes">&nbsp;(</xsl:text>
					<xsl:value-of select="$RefData/structures/ligues/ligue[@ID = $comite/@ligue]/nom"/>
					<xsl:text>)</xsl:text>
				</xsl:if>
			</xsl:when>

			<!-- Type 4 = Ligue -->
			<xsl:when test="$typeGroupe = '4'">
				<xsl:if test="$avecPrefixe = 'true'">
					<xsl:text disable-output-escaping="yes">Ligue</xsl:text>&nbsp;
				</xsl:if>
				<xsl:value-of select="$RefData/structures/ligues/ligue[@ID = $entiteId]/nom"/>
			</xsl:when>

			<!-- Type 5 ou 6 = Pays -->
			<xsl:when test="$typeGroupe = '5' or $typeGroupe = '6'">
				<xsl:value-of select="$RefData/structures/lesPays/pays[@ID = $entiteId]/@nom"/>
			</xsl:when>

			<!-- Fallback par defaut = Club -->
			<xsl:otherwise>
				<xsl:if test="$avecPrefixe = 'true'">
					<xsl:text disable-output-escaping="yes">Club</xsl:text>&nbsp;
				</xsl:if>
				<xsl:value-of select="$RefData/structures/clubs/club[@ID = $entiteId]/nom"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>