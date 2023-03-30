<?xml version="1.0"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:ms="urn:schemas-microsoft-com:xslt"
                xmlns:dt="urn:schemas-microsoft-com:datatypes">

	<xsl:output method="xml" indent="yes" />
	<xsl:param name="style"></xsl:param>

	<xsl:variable name="typeCompetition" select="/competition/@type"/>

	<xsl:template match="/">
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>



	<xsl:template match="/*">
		<head>
			<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
			<link rel="stylesheet" type="text/css" >
				<xsl:attribute name="href">
					file:///<xsl:value-of select="$style"/>style_relation_gr_ch.css
				</xsl:attribute>
			</link>
			<title>
				<xsl:value-of select="//epreuve[1]/@nom"/>
			</title>
		</head>
		<body>

			<div class="poule_title">
				<div id='poule_title_div'>
					<xsl:value-of select="//epreuve[1]/@nom"/>
				</div>
			</div>

			<div>
				<table class="t2">
					<tr>
						<th>
							NOM et Prénom
						</th>
						<th>
							Licence
						</th>
						<th style="text-align: right;">
							Grade
						</th>
						<th>
							C1
						</th>
						<th>
							C2
						</th>
						<th>
							C3
						</th>
						<th>
							C4
						</th>
						<th>
							C5
						</th>
						<th>
							C6
						</th>
						<th>
							C7
						</th>
						<th>
							C8
						</th>
						<th>
							Total
						</th>
					</tr>
					<xsl:apply-templates select="//epreuve/inscrits/judoka[@etat=4]">
						<xsl:sort select="@nom" order="ascending"/>
						<xsl:sort select="@classementFinal" data-type="number" order="ascending"/>
					</xsl:apply-templates>
				</table>
			</div>
		</body>
	</xsl:template>

	<xsl:template match="judoka">
		<xsl:variable name="club" select="@club"/>
		<xsl:variable name="comite" select="//club[@ID=$club]/@comite"/>
		<xsl:variable name="ligue" select="//club[@ID=$club]/@ligue"/>
		<xsl:variable name="position" select="position()"/>

		<xsl:variable name="grade" select="@grade"/>

		<tr>
			<xsl:attribute name="class">
				<xsl:if test="$position mod 2 = 0">
					<xsl:text>alter</xsl:text>
				</xsl:if>
			</xsl:attribute>
			<td>
				<xsl:value-of select="@nom"/>
				<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
				<xsl:value-of select="@prenom"/>
			</td>
			<td>
				<xsl:value-of select="@licence"/>
			</td>
			<td style="text-align: right;" class="tgrade">
				<xsl:choose>
					<xsl:when test= "//ceinture[@id=$grade]/@nom != ''">
						<xsl:value-of select="//ceinture[@id=$grade]/@nom"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<xsl:call-template name="combat" >
				<xsl:with-param name="judoka" select="@ID"/>
				<xsl:with-param name="position" select="1"/>
			</xsl:call-template>
			<td>
				<xsl:variable name="judoka" select="@ID"/>
				<xsl:value-of select="sum(//score[@judoka = $judoka]/@pointsGRCH)"/>

			</td>

		</tr>
	</xsl:template>

	<xsl:template name="combat">

		<xsl:param name="judoka"/>
		<xsl:param name="position"/>

		<td>
			<xsl:variable name="phase1">
				<xsl:for-each select="ancestor::epreuve[1]/phases/phase">
					<xsl:sort select="@id" data-type="number" order="ascending"/>
					<xsl:if test="position() = 1">
						<xsl:value-of select="@id"/>
					</xsl:if>
				</xsl:for-each>
			</xsl:variable>

			<xsl:variable name="count1" select="count(ancestor::epreuve[1]/phases/phase[@id = $phase1]/combats/combat[score[1]/@judoka = $judoka or score[2]/@judoka = $judoka])"/>


			<xsl:for-each select="ancestor::epreuve[1]/phases/phase">
				<xsl:sort select="@id" data-type="number" order="ascending"/>

				<xsl:if test="@typePhase = '1'">
					<!-- Liste des combats du judoka effectues (ne tient pas compte des combats non termines, vainqueur = -1 -->
					<xsl:for-each select="./combats/combat[(score[1]/@judoka = $judoka or score[2]/@judoka = $judoka) and @vainqueur != -1]">
						<xsl:sort select="@numero" data-type="number" order="ascending"/>

						<xsl:variable name="currentpos" >
							<xsl:if test="@phase = $phase1">
								<xsl:value-of select="position()"/>
							</xsl:if>
							<xsl:if test="@phase != $phase1">
								<xsl:value-of select="$count1 + position()"/>
							</xsl:if>
						</xsl:variable>

						<xsl:if test="$currentpos = $position">
							<xsl:if test="./score[1]/@judoka = $judoka">

								<xsl:call-template name="drawCombat" >
									<xsl:with-param name="combat" select="."/>
									<xsl:with-param name="participant1" select="./score[1]/@judoka"/>
								</xsl:call-template>
							</xsl:if>
							<xsl:if test="./score[2]/@judoka = $judoka">

								<xsl:call-template name="drawCombat" >
									<xsl:with-param name="combat" select="."/>
									<xsl:with-param name="participant1" select="./score[2]/@judoka"/>
								</xsl:call-template>
							</xsl:if>
						</xsl:if>
					</xsl:for-each>
				</xsl:if>

				<xsl:if test="@typePhase = '2'">
					<!-- Liste des combats du judoka effectues (ne tient pas compte des combats non termines, vainqueur = -1 -->
					<xsl:for-each select="./combats/combat[(score[1]/@judoka = $judoka or score[2]/@judoka = $judoka) and @virtuel = 'false' and @vainqueur != -1]">
						<xsl:sort select="@niveau" data-type="number" order="descending"/>

						<xsl:variable name="currentpos" >
							<xsl:if test="@phase = $phase1">
								<xsl:value-of select="position()"/>
							</xsl:if>
							<xsl:if test="@phase != $phase1">
								<xsl:value-of select="$count1 + position()"/>
							</xsl:if>
						</xsl:variable>

						<xsl:if test="$currentpos = $position">
							<xsl:if test="./score[1]/@judoka = $judoka">

								<xsl:call-template name="drawCombat" >
									<xsl:with-param name="combat" select="."/>
									<xsl:with-param name="participant1" select="./score[1]/@judoka"/>
								</xsl:call-template>
							</xsl:if>
							<xsl:if test="./score[2]/@judoka = $judoka">

								<xsl:call-template name="drawCombat" >
									<xsl:with-param name="combat" select="."/>
									<xsl:with-param name="participant1" select="./score[2]/@judoka"/>
								</xsl:call-template>
							</xsl:if>
						</xsl:if>

					</xsl:for-each>
				</xsl:if>
			</xsl:for-each>
		</td>
		<xsl:if test="$position &lt; 8">
			<xsl:call-template name="combat" >
				<xsl:with-param name="judoka" select="$judoka"/>
				<xsl:with-param name="position" select="$position + 1"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

	<xsl:template name="drawCombat">
		<xsl:param name="combat" />
		<xsl:param name="participant1" />

		<xsl:variable name="participant2">
			<xsl:if test= "$participant1 != $combat/score[1]/@judoka">
				<xsl:value-of select="$combat/score[1]/@judoka"/>
			</xsl:if>
			<xsl:if test= "$participant1 != $combat/score[2]/@judoka">
				<xsl:value-of select="$combat/score[2]/@judoka"/>
			</xsl:if>
		</xsl:variable>

		<table class="tscore">
			<tr>
				<td class="name">
					<xsl:if test= "$participant1!=@vainqueur">
						<xsl:text>X</xsl:text>
					</xsl:if>
					<xsl:if test= "$participant1=@vainqueur">
						<xsl:text>V</xsl:text>
					</xsl:if>
				</td>
				<td class="name">
					<xsl:if test= "$participant1 = $combat/score[1]/@judoka">
						<xsl:value-of select="$combat/score[1]/@pointsGRCH"/>
					</xsl:if>
					<xsl:if test= "$participant1 = $combat/score[2]/@judoka">
						<xsl:value-of select="$combat/score[2]/@pointsGRCH"/>
					</xsl:if>
				</td>
			</tr>
			<tr>
				<td colspan="2" class="name">
					<xsl:call-template name="score">
						<xsl:with-param name="combat" select="$combat"/>
					</xsl:call-template>
				</td>
			</tr>
		</table>
	</xsl:template>

	<xsl:template name="score">
		<xsl:param name="combat"/>
		<div class="score">
			<span>
				<xsl:value-of select="$combat/@scorevainqueur"/>
			</span>
			<xsl:if test="$typeCompetition != '1'">
				<span style="color:red;">
					<xsl:value-of select="$combat/@penvainqueur"/>
				</span>
			</xsl:if>
			<xsl:if test="$combat/@scorevainqueur != ''">
				<xsl:text disable-output-escaping="yes"> / </xsl:text>
			</xsl:if>
			<span>
				<xsl:value-of select="$combat/@scoreperdant"/>
			</span>
			<xsl:if test="$typeCompetition != '1'">
				<span style="color:red;">
					<xsl:value-of select="$combat/@penperdant"/>
				</span>
			</xsl:if>
		</div>
	</xsl:template>

</xsl:stylesheet>