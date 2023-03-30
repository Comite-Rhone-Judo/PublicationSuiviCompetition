<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" indent="yes" />
	<xsl:param name="style"></xsl:param>

	<xsl:variable name="typeCompetition" select="/competition/@type">
	</xsl:variable>

	<xsl:key name="niveau" match="combat" use="@niveau"/>

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
					file:///<xsl:value-of select="$style"/>style_feuille_resultat_shiai.css
				</xsl:attribute>
			</link>
			<title>
				<xsl:value-of select="//epreuve[1]/@nom"/>
			</title>
		</head>
		<body>
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
				</thead>
				<xsl:apply-templates select="//phase/poules/poule"/>
			</table>
		</body>
	</xsl:template>

	<xsl:template match="poule">
		<xsl:variable name="numero" select="@numero"/>

		<tr>
			<td>

				<div class="poule" style="page-break-inside : avoid;">
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
								<xsl:text disable-output-escaping="yes">&#032;(</xsl:text>
								<xsl:value-of select="//epreuve[1]/@nom"/>
								<xsl:text disable-output-escaping="yes">)</xsl:text>
							</div>
						</div>

						<xsl:if test="count(//combat[@reference=$numero and @niveau = 1]) &gt; 0">
							<xsl:call-template name="poule2" >
								<xsl:with-param name="poule" select="$numero"/>
								<xsl:with-param name="niveau" select="1"/>
							</xsl:call-template>
						</xsl:if>

						<xsl:if test="count(//combat[@reference=$numero and @niveau =2]) &gt; 0">
							<xsl:call-template name="poule2" >
								<xsl:with-param name="poule" select="$numero"/>
								<xsl:with-param name="niveau" select="2"/>
							</xsl:call-template>
						</xsl:if>

						<xsl:if test="count(//combat[@reference=$numero and @niveau =3]) &gt; 0">
							<xsl:call-template name="poule2" >
								<xsl:with-param name="poule" select="$numero"/>
								<xsl:with-param name="niveau" select="3"/>
							</xsl:call-template>
						</xsl:if>

						<xsl:if test="count(//combat[@reference=$numero and @niveau =4]) &gt; 0">
							<xsl:call-template name="poule2" >
								<xsl:with-param name="poule" select="$numero"/>
								<xsl:with-param name="niveau" select="4"/>
							</xsl:call-template>
						</xsl:if>


						<!--<hr class="filet" />-->
					</div>
				</div>
			</td>
		</tr>

	</xsl:template>


	<xsl:template name="poule2">
		<xsl:param name="poule" />
		<xsl:param name="niveau" />




		<div style="page-break-inside : avoid;">
			<table class="t2">
				<tr>
					<th>
						Nom et prénom
					</th>
					<!--<th>
            <xsl:variable name="ecartement" select="ancestor::phase[1]/@ecartement"/>
            <xsl:choose>
              <xsl:when test= "$ecartement = '3'">
                <xsl:text disable-output-escaping="yes">Comite</xsl:text>
              </xsl:when>

              <xsl:when test= "$ecartement = '4'">
                <xsl:text disable-output-escaping="yes">Ligue</xsl:text>
              </xsl:when>

              <xsl:otherwise>
                <xsl:text disable-output-escaping="yes">Club</xsl:text>
              </xsl:otherwise>
            </xsl:choose>
          </th>-->
					<th style="text-align: right;">Grade</th>
					<th style="text-align: right;"></th>
					<xsl:for-each select="//combat[@reference=$poule and @niveau = $niveau]">
						<!--<xsl:for-each select="key('niveau', @niveau)">-->
						<xsl:variable name="combat" select="."/>
						<xsl:variable name="participant1" select="$combat/score[1]/@judoka"/>
						<xsl:variable name="participant2" select="$combat/score[2]/@judoka"/>
						<th class="align_center">
							<xsl:for-each select="//participant[@poule=$poule]">
								<xsl:sort select="@position" data-type="number" order="ascending"/>
								<xsl:if test= "$participant1=@judoka">
									<xsl:value-of select="position()"/>
								</xsl:if>
							</xsl:for-each>
							-
							<xsl:for-each select="//participant[@poule=$poule]">
								<xsl:sort select="@position" data-type="number" order="ascending"/>
								<xsl:if test= "$participant2=@judoka">
									<xsl:value-of select="position()"/>
								</xsl:if>
							</xsl:for-each>
						</th>
					</xsl:for-each>
					<th>
						<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
						<xsl:text disable-output-escaping="yes">CUMUL</xsl:text>
						<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
					</th>
					<th>
						<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
						<xsl:text disable-output-escaping="yes">RESTANT</xsl:text>
						<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
					</th>
				</tr>
				<xsl:apply-templates select="//participant[@poule=$poule]">
					<xsl:sort select="@position" data-type="number" order="ascending"/>
					<xsl:with-param name="poule" select="$poule" />
					<xsl:with-param name="niveau" select="$niveau" />
				</xsl:apply-templates>
			</table>
		</div>


	</xsl:template>


	<xsl:template match="participant">
		<xsl:param name="poule" />
		<xsl:param name="niveau" />

		<xsl:variable name="participant1" select="@judoka" />
		<xsl:variable name="j1" select="//participants/participant[@judoka=$participant1]/descendant::*[1]" />

		<xsl:variable name="grade" select="$j1/@grade"/>
		<tr>
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

			<td style="text-align: right;" >
				<xsl:value-of select="position()"/>
			</td>

			<xsl:for-each select="//combat[@reference=$poule and @niveau = $niveau]">
				<xsl:variable name="combat" select="."/>

				<xsl:choose>
					<xsl:when test= "$combat/score[1]/@judoka=$participant1">
						<td class="tcombat">
							<xsl:apply-templates select="$combat">
								<xsl:with-param name="participant1" select="$participant1" />
							</xsl:apply-templates>
						</td>
					</xsl:when>
					<xsl:when test= "$combat/score[2]/@judoka=$participant1">
						<td class="tcombat">
							<xsl:apply-templates select="$combat">
								<xsl:with-param name="participant1" select="$participant1" />
							</xsl:apply-templates>
						</td>
					</xsl:when>
					<xsl:otherwise>
						<td class="tcombatsilver"></td>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>

			<td class="align_center tresult">
				<xsl:value-of select="@cumulPointsGRCH"/>
			</td>
			<td class="align_center tresult">
				<xsl:value-of select="$j1/@corg"/>
			</td>
		</tr>
	</xsl:template>



	<!--<xsl:template match="combat[@vainqueur = 0]">
    <table class="tscore">
      <tr>
        <td class="name tcombatsilver3">
          <xsl:text> </xsl:text>
        </td>
      </tr>
    </table>
  </xsl:template>-->


	<xsl:template match="combat[@vainqueur != -1]">
		<xsl:param name="participant1" />

		<table class="tscore">
			<tr>
				<td class="name">
					<xsl:if test= "$participant1 != @vainqueur">
						<xsl:attribute name="class">
							tcombatsilver2
						</xsl:attribute>
						<xsl:if test= "@vainqueur != 0">
							<xsl:text>X</xsl:text>
						</xsl:if>
					</xsl:if>
					<xsl:if test= "$participant1 = @vainqueur">
						<span>
							<xsl:value-of select="./@scorevainqueur"/>
						</span>

						<xsl:if test="$typeCompetition != '1'">
							<span style="color:red;">
								<xsl:value-of select="./@penvainqueur"/>
							</span>
						</xsl:if>
					</xsl:if>
				</td>
			</tr>
			<xsl:if test= "$participant1=@vainqueur">
				<tr>
					<td colspan="2" class="name">
						<span>
							<xsl:value-of select="./@scoreperdant"/>
						</span>
						<xsl:if test="$typeCompetition != '1'">
							<span style="color:red;">
								<xsl:value-of select="./@penperdant"/>
							</span>
						</xsl:if>

					</td>
				</tr>
			</xsl:if>
		</table>
	</xsl:template>

</xsl:stylesheet>