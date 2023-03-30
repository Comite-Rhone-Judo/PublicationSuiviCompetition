<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" indent="yes" />
	<xsl:param name="style"></xsl:param>

	<xsl:variable name="typeCompetition" select="/competition/@type">
	</xsl:variable>

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
					file:///<xsl:value-of select="$style"/>style_feuille_poule.css
				</xsl:attribute>
			</link>


			<title>
				<xsl:value-of select="//epreuve[1]/@nom"/>
			</title>
		</head>
		<body>


			<div>
				<table class="t2">
					<thead>
						<tr>
							<th colspan="6" >
								<!--<div style="text-align:center;">
                  <xsl:text disable-output-escaping="no"> Epreuve : </xsl:text>
                  <xsl:value-of select="//epreuve[1]/@nom" />
                </div>-->

								<div style="float:left; left:0px;">
									<xsl:value-of select="count(//participant)"/>
									<xsl:text disable-output-escaping="yes">&#32;judokas.</xsl:text>
								</div>

								<div style="float:right; right:0px;">
									<xsl:variable name="date" select="//phase[1]/@date_tirage"/>
									<xsl:variable name="time" select="//phase[1]/@time_tirage"/>

									<xsl:text disable-output-escaping="yes">Tirage&#32;:&#32;</xsl:text>
									<xsl:value-of select="concat(substring($date, 1, 2), '-', substring($date, 3, 2), '-',substring($date, 5))" />
									<xsl:text>&#32;</xsl:text>
									<xsl:value-of select="concat(substring($time, 1, 2), ':', substring($time, 3, 2), ':',substring($time, 5))" />
								</div>
							</th>
						</tr>
						<tr>
							<th>N°</th>
							<th>
								NOM et Prénom
							</th>
							<th>
								Club
							</th>
							<th>
								Comité
							</th>
							<th>
								Ligue
							</th>
							<th>
								Pays
							</th>
						</tr>
					</thead>
					<xsl:apply-templates select="//participant">
						<xsl:sort select="./judoka/@nom"/>
					</xsl:apply-templates>
				</table>
			</div>
			<!--<xsl:apply-templates select="//phase/poules/poule"/>-->

		</body>
	</xsl:template>

	<xsl:template match="poule">
		<xsl:variable name="numero" select="@numero"/>
		<div class="poule" style="page-break-inside: avoid;" >
			<div>

				<div class="poule_title">

					<div id='poule_heure_div'>
						<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
					</div>
					<div id='poule_tapis_div'>
						Tapis <xsl:value-of select="../../combats/combat[@reference=$numero][1]/@tapis"/>
					</div>
					<div id='poule_title_div'>
						<xsl:value-of select="@nom"/>
					</div>
				</div>


				<div>
					<table class="t2">
						<tr>
							<th></th>
							<th>
								NOM et Prénom
							</th>
							<th>
								Club
							</th>
							<th>
								Comité
							</th>
							<th>
								Ligue
							</th>
							<th>
								Pays
							</th>

						</tr>
						<xsl:apply-templates select="//participant[@poule=$numero]">
							<xsl:with-param name="poule" select="$numero" />
						</xsl:apply-templates>
					</table>
				</div>
			</div>
		</div>
	</xsl:template>


	<xsl:template match="participant">
		<xsl:param name="poule" />

		<xsl:variable name="participant1" select="@judoka" />
		<xsl:variable name="j1" select="//participants/participant[@judoka=$participant1]/descendant::*[1]" />

		<xsl:variable name="position" select="position()"/>
		<xsl:variable name="club" select="$j1/@club"/>
		<!--<xsl:variable name="comite" select="//club[@ID=$club]/@comite"/>
    <xsl:variable name="ligue" select="//club[@ID=$club]/@ligue"/>-->

		<xsl:variable name="comite" >
			<xsl:if test="$typeCompetition = '1'">
				<xsl:value-of select="$j1/@comite"/>
			</xsl:if>
			<xsl:if test="$typeCompetition != '1'">
				<xsl:value-of select="//club[@ID=$club]/@comite"/>
			</xsl:if>
		</xsl:variable>

		<xsl:variable name="ligue" >
			<xsl:if test="$typeCompetition = '1'">
				<xsl:value-of select="$j1/@ligue"/>
			</xsl:if>
			<xsl:if test="$typeCompetition != '1'">
				<xsl:value-of select="//club[@ID=$club]/@ligue"/>
			</xsl:if>
		</xsl:variable>

		<tr>
			<xsl:attribute name="class">
				<xsl:if test="$position mod 2 = 0">
					<xsl:text>alter</xsl:text>
				</xsl:if>
			</xsl:attribute>

			<td class="align_center">
				<xsl:text  disable-output-escaping="yes">P</xsl:text>
				<xsl:value-of select="@poule"/>
			</td>
			<td>
				<xsl:value-of select="$j1/@nom"/>
				<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
				<xsl:value-of select="$j1/@prenom"/>
				<xsl:if test="$typeCompetition = '1'">
					<xsl:if test="$j1/judoka/@serie != '0'">
						<xsl:text disable-output-escaping="yes">&#032;(TS</xsl:text>
						<xsl:value-of select="$j1/judoka/@serie"/>
						<xsl:text disable-output-escaping="yes">)</xsl:text>
					</xsl:if>
				</xsl:if>
				<xsl:if test="$typeCompetition != '1'">
					<xsl:if test="$j1/@serie != '0'">
						<xsl:text disable-output-escaping="yes">&#032;(TS</xsl:text>
						<xsl:value-of select="$j1/@serie"/>
						<xsl:text disable-output-escaping="yes">)</xsl:text>
					</xsl:if>
				</xsl:if>
			</td>
			<td>
				<xsl:value-of select="//club[@ID=$club]/nom"/>
			</td>
			<td>
				<xsl:value-of select="$comite"/>
			</td>
			<td>
				<xsl:value-of select="//ligue[@ID=$ligue]/nomCourt"/>
			</td>
			<td>
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>