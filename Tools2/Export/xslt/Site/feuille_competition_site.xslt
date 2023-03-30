<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"></xsl:param>
	<!--<xsl:param name="menu"></xsl:param>-->
	<xsl:param name="js"></xsl:param>

	<xsl:key name="combats" match="combat" use="@niveau"/>

	<xsl:variable name="typeCompetition" select="/competition[1]/@type">
	</xsl:variable>

	<xsl:template match="/">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>

	<xsl:template match="/*">
		<head>
			<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
			<meta name="viewport" content="width=device-width, initial-scale=1.0, shrink-to-fit=no" />
			<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
			<meta http-equiv="Pragma" content="no-cache" />
			<meta http-equiv="Expires" content="0" />

			<link type="text/css" rel="stylesheet" href="../style/style_feuille_tableau.css" ></link>
			<link type="text/css" rel="stylesheet" href="../style/style_menu.css" ></link>

			<!--<script src="../js/jquery.min.js"></script>
      <script src="../js/script.js"></script>-->

			<script type="text/javascript" >
				<xsl:value-of select="$js"/>
			</script>

			<!--<xsl:variable name="css1">
        file:///<xsl:value-of select="$style"/>
        <xsl:text>style_feuille_tableau.css</xsl:text>
      </xsl:variable>
      <xsl:variable name="css2">
        file:///<xsl:value-of select="$style"/>
        <xsl:text>style_menu.css</xsl:text>
      </xsl:variable>

      <style type="text/css">
        <xsl:value-of select="document($css1)" disable-output-escaping="yes" />
        <xsl:value-of select="document($css2)" disable-output-escaping="yes" />
      </style>

      <script type="text/javascript" >
        <xsl:value-of select="$js"/>
      </script>-->

			<title>
				<xsl:value-of select="//epreuve[1]/@nom"/>
			</title>
		</head>
		<body>
			<!--<div class="bandeau">
        <div class="header1" >
          <xsl:value-of select="./titre"/>
          <xsl:text> - </xsl:text>
          <xsl:value-of select="./lieu"/>
        </div>
        <div class="header2" >
          <xsl:value-of select="//epreuve[1]/@nom_cateage"/>
          <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
          <xsl:value-of select="//epreuve[1]/@nom"/>
          <xsl:value-of select="//epreuve[1]/@sexe"/>
        </div>

      </div>-->

			<div class="btn_defilement">
				<div style="position: fixed; top: 10px; right:10px;">
					<a class="btn btn-danger" onclick="anim2();"  style="margin-right: 10px;">Actualiser</a>
					<a class="btn btn-info" onclick="setDefilement();">Défilement</a>
				</div>
			</div>

			<div class="btn_menu">
				<a class="btn btn-warning" href="../common/menu.html" role="button">Menu</a>
			</div>

			<div class="panel panel-primary">
				<div class="panel-heading clearfix" style="text-align:center;">

					<span style="font-size:1.0em;">
						<xsl:value-of select="./titre"/>
						<xsl:text> - </xsl:text>
						<xsl:value-of select="./lieu"/>
					</span>
					<br/>
					<span style="font-size:1.2em;">
						<!--<xsl:text> (</xsl:text>-->
						<!--<xsl:value-of select="//epreuve[1]/@nom_cateage"/>
                      <xsl:text> </xsl:text>-->
						<xsl:value-of select="//epreuve[1]/@nom"/>
						<!--<xsl:text>)</xsl:text>-->
					</span>

				</div>

				<div class="panel-body">
					<div class="col-md-12" style="padding:0px;">
						<div class="panel panel-info" style="text-align:center;">

							<div class="panel-heading clearfix">
								<span style="width: 100%;font-weight: 600;">
									TABLEAU PRINCIPAL
								</span>
							</div>

							<div class="panel-body">
								<xsl:variable name="repechage">
									<xsl:text>false</xsl:text>
								</xsl:variable>

								<xsl:call-template name="tableau">
									<xsl:with-param name="repechage" select="$repechage">
									</xsl:with-param>
								</xsl:call-template>
							</div>
						</div>
					</div>

					<div class="col-md-12" style="padding:0px;">
						<div class="panel panel-info" style="text-align:center;">

							<div class="panel-heading clearfix">
								<span style="width: 100%;font-weight: 600;">
									TABLEAU REPECHAGE
								</span>
							</div>

							<div class="panel-body">
								<xsl:variable name="repechage1">
									<xsl:text>true</xsl:text>
								</xsl:variable>

								<xsl:call-template name="tableau">
									<xsl:with-param name="repechage" select="$repechage1">
									</xsl:with-param>
								</xsl:call-template>
							</div>
						</div>
					</div>

				</div>
			</div>


			<!--<div class="main">

        <div class="header1" >
          TABLEAU PRINCIPAL
        </div>
        <xsl:variable name="repechage">
          <xsl:text>false</xsl:text>
        </xsl:variable>

        <xsl:call-template name="tableau">
          <xsl:with-param name="repechage" select="$repechage">
          </xsl:with-param>
        </xsl:call-template>

        <div class="header1" >
          TABLEAU REPECHAGE
        </div>
        <xsl:variable name="repechage1">
          <xsl:text>true</xsl:text>
        </xsl:variable>

        <xsl:call-template name="tableau">
          <xsl:with-param name="repechage" select="$repechage1">
          </xsl:with-param>
        </xsl:call-template>

      </div>-->

		</body>
	</xsl:template>


	<xsl:template name="tableau">
		<xsl:param name="repechage" />

		<xsl:variable name="niveau">
			<xsl:for-each select="//combat[@repechage=$repechage and generate-id() = generate-id(key('combats',@niveau)[1])]">
				<xsl:sort select="@niveau" data-type="number" order="descending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:variable name="niveaumax">
			<xsl:for-each select="//combat[@repechage=$repechage and generate-id() = generate-id(key('combats',@niveau)[1])]">
				<xsl:sort select="@niveau" data-type="number" order="ascending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:variable name="niveaumin">
			<xsl:for-each select="//combat[@repechage=$repechage and generate-id() = generate-id(key('combats',@niveau)[1])]">
				<xsl:sort select="@niveau" data-type="number" order="descending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<!--<xsl:variable name="niveau">-->
		<!--<xsl:for-each select="//combat[@repechage=$repechage and generate-id() = generate-id(key('combats',@niveau)[1])]">
      <xsl:sort select="@niveau" data-type="number" order="descending"/>
      <xsl:variable name="niveau" select="@niveau"></xsl:variable>-->

		<table class="t1" style="page-break-after: always;">

			<!-- BOUCLE SUR LES COMBATS DU NIVEAU EN COURS -->
			<xsl:for-each select="//combat[@niveau=$niveau and @repechage=$repechage]">
				<xsl:sort select="@reference" order="ascending"/>
				<tr>
					<xsl:if test="position() !=  1 and (position() -  1) mod 8 = 0">
						<xsl:attribute name="style">page-break-before : always;</xsl:attribute>
					</xsl:if>
					<xsl:apply-templates select=".">
						<xsl:with-param name="recursion" select="0" />
						<xsl:with-param name="position" select="position()" />
						<xsl:with-param name="repechage" select="$repechage" />
						<xsl:with-param name="rowspan1" select="0" />
						<xsl:with-param name="niveauPrev" select="0" />
						<xsl:with-param name="countNiveauPrev" select="0" />
						<xsl:with-param name="niveaumax" select="$niveaumax" />
						<xsl:with-param name="niveaumin" select="$niveaumin" />

					</xsl:apply-templates>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>

	<xsl:template match="combat">
		<!-- DEFINITION DES VARIABLES -->

		<xsl:param name="recursion" />
		<xsl:param name="position" />
		<xsl:param name="repechage" />
		<xsl:param name="rowspan1" />
		<xsl:param name="niveauPrev" />
		<xsl:param name="countNiveauPrev" />
		<xsl:param name="niveaumax" />
		<xsl:param name="niveaumin" />

		<xsl:variable name="p">
			<xsl:call-template name="power">
				<xsl:with-param name="base" select="2"/>
				<xsl:with-param name="power" select="$rowspan1"/>
			</xsl:call-template>
		</xsl:variable>

		<xsl:variable name="niveau" select="@niveau" />
		<xsl:variable name="countNiveau" select="count(//combat[@niveau = $niveau and  @repechage=$repechage])" />

		<xsl:variable name="niveauNext" >
			<xsl:for-each select="//combat[@niveau &lt;= $niveau and  @repechage=$repechage and generate-id() = generate-id(key('combats',@niveau)[1])]">
				<xsl:sort select="@niveau" data-type="number" order="descending"/>
				<xsl:if test="position() = 2">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="countNiveauNext" select="count(//combat[@niveau = $niveauNext and  @repechage=$repechage])" />


		<!-- AFFICHAGE DU COMBAT -->
		<td>
			<xsl:if test= "($position - 1) mod $p = 0">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="$p"/>
				</xsl:attribute>
			</xsl:if>
			<!--<xsl:if test= "$recursion = 2">
        <xsl:attribute name="style">
          page-break-inside: avoid;
        </xsl:attribute>
      </xsl:if>-->

			<xsl:call-template name="combat2">
				<xsl:with-param name="combat" select="."/>
				<xsl:with-param name="rowspan" select="$p"/>
				<xsl:with-param name="niveaumax" select="$niveaumin"/>
				<xsl:with-param name="left">
					<xsl:text>true</xsl:text>
				</xsl:with-param>
				<xsl:with-param name="decroch">
					<xsl:if test= "$countNiveauPrev != $countNiveau">
						<xsl:text>false</xsl:text>
					</xsl:if>
					<xsl:if test= "$countNiveauPrev = $countNiveau">
						<xsl:text>true</xsl:text>
					</xsl:if>

				</xsl:with-param>

			</xsl:call-template>
		</td>
		<td style="width:10px">
			<xsl:if test= "($position - 1) mod $p = 0">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="$p"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:attribute name="style">
				width:10px;height:<xsl:value-of select="200*$p"/>px;
			</xsl:attribute>
			<div class="line">
				<table>
					<tr>
						<td>

						</td>
					</tr>
					<tr >
						<td class="line1">
							<xsl:if test= "$countNiveauPrev != $countNiveau">
								<xsl:attribute name="rowspan">2</xsl:attribute>
							</xsl:if>
							<xsl:if test= "$countNiveauPrev = $countNiveau">
								<xsl:attribute name="rowspan">3</xsl:attribute>
							</xsl:if>
							<div class="arrondi"></div>
						</td>
						<td class="line2">
						</td>
					</tr>
					<tr>
						<td>
						</td>
					</tr>
					<tr>
						<td>
						</td>
					</tr>

				</table>
			</div>
		</td>

		<xsl:if test= "$niveau = $niveaumax">
			<td>
				<xsl:attribute name="style">
					height:<xsl:value-of select="200*$p"/>px;vertical-align:middle;
				</xsl:attribute>
				<xsl:if test= "($position - 1) mod $p = 0">
					<xsl:attribute name="rowspan">
						<xsl:value-of select="$p"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:call-template name="combatVainqueur">
					<xsl:with-param name="combat" select="."/>
				</xsl:call-template>
			</td>
		</xsl:if>

		<!-- RECURSIVITE -->
		<!--<xsl:if test= "($niveauNext != 0 and $niveauNext != $niveau) ">-->
		<!--and $recursion != 2-->

		<!-- CALCUL DES NOUVELLES VARIABLES -->
		<xsl:variable name="p1">
			<xsl:if test= "$countNiveauNext != $countNiveau">
				<xsl:call-template name="power">
					<xsl:with-param name="base" select="2"/>
					<xsl:with-param name="power" select="($recursion + 1)"/>
				</xsl:call-template>
			</xsl:if>

			<xsl:if test= "$countNiveauNext = $countNiveau">
				<xsl:value-of select="$p"/>
			</xsl:if>
		</xsl:variable>

		<xsl:variable name="rowspan2">
			<xsl:if test= "$countNiveauNext != $countNiveau">
				<xsl:value-of select="$rowspan1 + 1"/>
			</xsl:if>

			<xsl:if test= "$countNiveauNext = $countNiveau">
				<xsl:value-of select="$rowspan1"/>
			</xsl:if>
		</xsl:variable>


		<xsl:variable name="p3">
			<!--<xsl:choose>
        <xsl:when test= "$countNiveauNext = $countNiveau">
          <xsl:value-of select="$position"/>
        </xsl:when>
        <xsl:when test= "$countNiveauNext != $countNiveau">-->
			<xsl:value-of select="(($position - 1) div $p1) + 1"/>
			<!--</xsl:when>
      </xsl:choose>-->
		</xsl:variable>

		<xsl:for-each select="//combat[@niveau=$niveauNext and  @repechage=$repechage]">
			<xsl:sort select="@reference" order="ascending"/>
			<xsl:if test="position() = $p3">
				<xsl:apply-templates select=".">
					<xsl:with-param name="recursion" >

						<xsl:if test= "$countNiveauNext != $countNiveau">
							<xsl:value-of select="$recursion + 1"/>
						</xsl:if>
						<xsl:if test= "$countNiveauNext = $countNiveau">
							<xsl:value-of select="$recursion"/>
						</xsl:if>

					</xsl:with-param>
					<xsl:with-param name="position" select="$position" />
					<xsl:with-param name="repechage" select="$repechage"/>
					<xsl:with-param name="rowspan1" select="$rowspan2"/>
					<xsl:with-param name="countNiveauPrev" select="$countNiveau"/>
					<xsl:with-param name="niveauPrev" select="$niveau"/>
					<xsl:with-param name="niveaumax" select="$niveaumax"/>
				</xsl:apply-templates>
			</xsl:if>
		</xsl:for-each>
		<!--</xsl:if>-->
	</xsl:template>



	<xsl:template name="combat2">
		<xsl:param name="combat"/>
		<xsl:param name="niveaumax"/>
		<xsl:param name="rowspan"/>
		<xsl:param name="left"/>
		<xsl:param name="last"/>
		<xsl:param name="decroch"/>

		<xsl:variable name="participant1" select="$combat/score[1]/@judoka"/>
		<xsl:variable name="judoka1" select="//participant[@judoka=$participant1]/descendant::*[1]"/>
		<xsl:variable name="club1" select="$judoka1/@club"/>
		<xsl:variable name="comite1" select="//club[@ID=$club1]/@comite"/>
		<xsl:variable name="ligue1" select="//club[@ID=$club1]/@ligue"/>

		<xsl:variable name="participant2" select="$combat/score[2]/@judoka"/>
		<xsl:variable name="judoka2" select="//participant[@judoka=$participant2]/descendant::*[1]"/>
		<xsl:variable name="club2" select="$judoka2/@club"/>
		<xsl:variable name="comite2" select="//club[@ID=$club2]/@comite"/>
		<xsl:variable name="ligue2" select="//club[@ID=$club2]/@ligue"/>

		<div class="d1">
			<xsl:attribute name="style">
				height:<xsl:value-of select="200*$rowspan"/>px;
				<xsl:if test= "$decroch = 'true'">
					position: relative;
					top: 20px;
				</xsl:if>
			</xsl:attribute>
			<table class="combat">
				<xsl:if test= "$last = 'true'">
					<tr></tr>
				</xsl:if>
				<tr>
					<td colspan="4" class="combatname" >
						<!--<xsl:if test= "//participants/participant[@judoka=$participant1]/descendant::*[1]/@nom">
              <xsl:if test= "$combat/@niveau = $niveaumax">
                <div>
                  <xsl:if test= "$left = 'true'">
                    <xsl:attribute name="class">left_club_top</xsl:attribute>
                  </xsl:if>
                  <xsl:if test= "$left = 'false'">
                    <xsl:attribute name="class">right_club_top</xsl:attribute>
                  </xsl:if>
                  <span>
                    (<xsl:value-of select="//club[@ID=//participants/participant[@judoka=$participant1]/descendant::*[1]/@club]/nomCourt"/>)
                  </span>
                </div>
              </xsl:if>
            </xsl:if>-->

						<xsl:variable name="class" >
							<xsl:if test= "$left = 'true'">
								left_line alert alert-warning
							</xsl:if>
							<xsl:if test= "$left = 'false'">
								right_line alert alert-warning
							</xsl:if>
						</xsl:variable>

						<xsl:choose>
							<xsl:when test= "$judoka1/@nom">

								<div>
									<xsl:attribute name="class">
										<xsl:value-of select="$class"/>
									</xsl:attribute>

									<div>
										<span>
											<xsl:value-of select="$judoka1/@nom"/>
											<xsl:text disable-output-escaping="yes">&#160;</xsl:text>

											<xsl:if test= "$combat/@niveau != $niveaumax">
												<xsl:value-of select="substring ($judoka1/@prenom, 1,1)"/>
												<xsl:text disable-output-escaping="yes">.</xsl:text>
											</xsl:if>

											<xsl:if test= "$combat/@niveau = $niveaumax">
												<xsl:value-of select="$judoka1/@prenom"/>
											</xsl:if>
										</span>
									</div>

									<div>
										<xsl:if test= "$combat/@niveau = $niveaumax">

											<span class="left_club_bottom">
												<xsl:variable name="ecartement1" select="//phase[@id=$combat/phase]/@ecartement"/>
												<xsl:choose>
													<xsl:when test= "$ecartement1 = '3'">
														<xsl:if test="$typeCompetition != 1">
															<xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
															<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
															<xsl:value-of select="$comite1"/>
														</xsl:if>
														<xsl:if test="$typeCompetition = 1">
															<xsl:value-of select="$comite1"/>
														</xsl:if>
													</xsl:when>

													<xsl:when test= "$ecartement1 = '4'">
														<xsl:if test="$typeCompetition != 1">
															<xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
															<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
															<xsl:value-of select="//ligue[@ID=$ligue1]/nomCourt"/>
														</xsl:if>
														<xsl:if test="$typeCompetition = 1">
															<xsl:value-of select="//ligue[@ID=$ligue1]/nomCourt"/>
														</xsl:if>
													</xsl:when>

													<xsl:otherwise>
														<xsl:if test="$typeCompetition != 1">
															<xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
															<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
															<xsl:value-of select="$comite1"/>
														</xsl:if>
														<xsl:if test="$typeCompetition = 1">
															<xsl:value-of select="$comite1"/>
														</xsl:if>
													</xsl:otherwise>
												</xsl:choose>
											</span>

											<!--(<xsl:value-of select="//club[@ID=//participants/participant[@judoka=$participant1]/descendant::*[1]/@club]/nomCourt"/>)-->
											<!--</span>
                      </div>-->
										</xsl:if>
									</div>

								</div>



								<xsl:variable name="ref" select="$combat/feuille/@ref1"/>
								<xsl:variable name="combat_prec" select="//combat[@reference = $ref]">
								</xsl:variable>

								<xsl:call-template name="score">
									<xsl:with-param name="combat" select="$combat_prec"/>
								</xsl:call-template>

							</xsl:when>
							<xsl:otherwise>
								<div>
									<xsl:attribute name="class">
										<xsl:value-of select="$class"/>
									</xsl:attribute>
								</div>
							</xsl:otherwise>
						</xsl:choose>

						<!--<div>

              <xsl:if test= "$judoka1/@nom">
                <xsl:if test= "$combat/@niveau = $niveaumax">
                  <span class="left_club_bottom">
                    <xsl:text disable-output-escaping="yes">&#032;(</xsl:text>

                    <xsl:variable name="ecartement1" select="//phase[@id=$combat/phase]/@ecartement"/>
                    <xsl:choose>
                      <xsl:when test= "$ecartement1 = '3'">
                        <xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
                        <xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
                        <xsl:value-of select="$comite1"/>
                      </xsl:when>

                      <xsl:when test= "$ecartement1 = '4'">
                        <xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
                        <xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
                        <xsl:value-of select="//ligue[@ID=$ligue1]/nomCourt"/>
                      </xsl:when>

                      <xsl:otherwise>
                        <xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
                        <xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
                        <xsl:value-of select="$comite1"/>
                      </xsl:otherwise>
                    </xsl:choose>
                    <xsl:text disable-output-escaping="yes">)</xsl:text>
                  </span>
                </xsl:if>

                <span>
                  <xsl:value-of select="$judoka1/@nom"/>
                  <xsl:text disable-output-escaping="yes">&#160;</xsl:text>

                  <xsl:if test= "$combat/@niveau != $niveaumax">
                    <xsl:value-of select="substring ($judoka1/@prenom, 1,1)"/>
                    <xsl:text disable-output-escaping="yes">.</xsl:text>
                  </xsl:if>

                  <xsl:if test= "$combat/@niveau = $niveaumax">
                    <xsl:value-of select="$judoka1/@prenom"/>
                  </xsl:if>
                </span>

                <xsl:variable name="ref" select="$combat/feuille/@ref1"/>

                <xsl:variable name="combat_prec" select="//combat[@reference = $ref]">
                </xsl:variable>

                <xsl:call-template name="score">
                  <xsl:with-param name="combat" select="$combat_prec"/>
                </xsl:call-template>
              </xsl:if>
            </div>-->
					</td>
				</tr>

				<tr>
					<td colspan="4" class="combatname" >
						<xsl:if test= "$decroch = 'true'">
							<xsl:attribute name="style">vertical-align: bottom;</xsl:attribute>
						</xsl:if>

						<xsl:variable name="class" >
							<xsl:if test= "$left = 'true' and $last = 'true'">
								right_line alert alert-warning
							</xsl:if>
							<xsl:if test= "$left = 'true' and $last != 'true'">
								left_line alert alert-warning
							</xsl:if>
							<xsl:if test= "$left = 'false'">
								right_line alert alert-warning
							</xsl:if>
						</xsl:variable>

						<xsl:choose>
							<xsl:when test= "$judoka2/@nom">

								<xsl:variable name="ref" select="$combat/feuille/@ref2"/>

								<xsl:variable name="combat_prec" select="//combat[@reference = $ref]">
								</xsl:variable>

								<xsl:call-template name="score">
									<xsl:with-param name="combat" select="$combat_prec"/>
								</xsl:call-template>

								<div>
									<xsl:attribute name="class">
										<xsl:value-of select="$class"/>
									</xsl:attribute>



									<div>
										<xsl:if test= "$combat/@niveau = $niveaumax">

											<span class="left_club_bottom">

												<xsl:variable name="ecartement2" select="//phase[@id=$combat/phase]/@ecartement"/>
												<xsl:choose>
													<xsl:when test= "$ecartement2 = '3'">
														<xsl:if test="$typeCompetition != 1">
															<xsl:value-of select="//club[@ID=$club2]/nomCourt"/>
															<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
															<xsl:value-of select="$comite2"/>
														</xsl:if>
														<xsl:if test="$typeCompetition = 1">
															<xsl:value-of select="$comite2"/>
														</xsl:if>
													</xsl:when>

													<xsl:when test= "$ecartement2 = '4'">
														<xsl:if test="$typeCompetition != 1">
															<xsl:value-of select="//club[@ID=$club2]/nomCourt"/>
															<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
															<xsl:value-of select="//ligue[@ID=$ligue2]/nomCourt"/>
														</xsl:if>
														<xsl:if test="$typeCompetition = 1">
															<xsl:value-of select="//ligue[@ID=$ligue2]/nomCourt"/>
														</xsl:if>
													</xsl:when>

													<xsl:otherwise>
														<xsl:if test="$typeCompetition != 1">
															<xsl:value-of select="//club[@ID=$club2]/nomCourt"/>
															<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
															<xsl:value-of select="$comite2"/>
														</xsl:if>
														<xsl:if test="$typeCompetition = 1">
															<xsl:value-of select="$comite2"/>
														</xsl:if>
													</xsl:otherwise>
												</xsl:choose>
											</span>
										</xsl:if>
									</div>

									<div>
										<span >
											<xsl:if test= "$decroch = 'true'">
												<xsl:attribute name="style">top: -1.0em;</xsl:attribute>
											</xsl:if>
											<xsl:if test= "$decroch = 'false'">
												<xsl:attribute name="style">top: -0.5em;</xsl:attribute>
											</xsl:if>
											<xsl:value-of select="$judoka2/@nom"/>
											<xsl:text disable-output-escaping="yes">&#160;</xsl:text>

											<xsl:if test= "$combat/@niveau != $niveaumax">
												<xsl:value-of select="substring ($judoka2/@prenom, 1,1)"/>
												<xsl:text disable-output-escaping="yes">.</xsl:text>
											</xsl:if>

											<xsl:if test= "$combat/@niveau = $niveaumax">
												<xsl:value-of select="$judoka2/@prenom"/>
											</xsl:if>

										</span>
									</div>

								</div>
							</xsl:when>
							<xsl:otherwise>
								<div>
									<xsl:attribute name="class">
										<xsl:value-of select="$class"/>
									</xsl:attribute>
								</div>
							</xsl:otherwise>

						</xsl:choose>
						<!--<div>
              <xsl:if test= "$left = 'true' and $last = 'true'">
                <xsl:attribute name="class">right_line alert alert-success</xsl:attribute>
              </xsl:if>
              <xsl:if test= "$left = 'true' and $last != 'true'">
                <xsl:attribute name="class">left_line alert alert-success</xsl:attribute>
              </xsl:if>
              <xsl:if test= "$left = 'false'">
                <xsl:attribute name="class">right_line alert alert-success</xsl:attribute>
              </xsl:if>

              <xsl:if test= "$judoka2/@nom">

                <xsl:if test= "$combat/@niveau = $niveaumax">
                  <span class="left_club_bottom">
                    <xsl:text disable-output-escaping="yes">&#032;(</xsl:text>

                    <xsl:variable name="ecartement2" select="//phase[@id=$combat/phase]/@ecartement"/>
                    <xsl:choose>
                      <xsl:when test= "$ecartement2 = '3'">
                        <xsl:value-of select="//club[@ID=$club2]/nomCourt"/>
                        <xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
                        <xsl:value-of select="$comite2"/>
                      </xsl:when>

                      <xsl:when test= "$ecartement2 = '4'">
                        <xsl:value-of select="//club[@ID=$club2]/nomCourt"/>
                        <xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
                        <xsl:value-of select="//ligue[@ID=$ligue2]/nomCourt"/>
                      </xsl:when>

                      <xsl:otherwise>
                        <xsl:value-of select="//club[@ID=$club2]/nomCourt"/>
                        <xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
                        <xsl:value-of select="$comite2"/>
                      </xsl:otherwise>
                    </xsl:choose>
                    <xsl:text disable-output-escaping="yes">)</xsl:text>                    
                  </span>
                </xsl:if>

                <span >
                  <xsl:if test= "$decroch = 'true'">
                    <xsl:attribute name="style">top: -1.0em;</xsl:attribute>
                  </xsl:if>
                  <xsl:if test= "$decroch = 'false'">
                    <xsl:attribute name="style">top: -0.5em;</xsl:attribute>
                  </xsl:if>
                  <xsl:value-of select="$judoka2/@nom"/>
                  <xsl:text disable-output-escaping="yes">&#160;</xsl:text>

                  <xsl:if test= "$combat/@niveau != $niveaumax">
                    <xsl:value-of select="substring ($judoka2/@prenom, 1,1)"/>
                    <xsl:text disable-output-escaping="yes">.</xsl:text>
                  </xsl:if>

                  <xsl:if test= "$combat/@niveau = $niveaumax">
                    <xsl:value-of select="$judoka2/@prenom"/>
                  </xsl:if>

                </span>

                <xsl:variable name="ref" select="$combat/feuille/@ref2"/>

                <xsl:variable name="combat_prec" select="//combat[@reference = $ref]">
                </xsl:variable>

                <xsl:call-template name="score">
                  <xsl:with-param name="combat" select="$combat_prec"/>
                </xsl:call-template>

              </xsl:if>
            </div>-->

						<!--<xsl:if test= "//participants/participant[@judoka=$participant2]/descendant::*[1]/@nom">
              <xsl:if test= "$combat/@niveau = $niveaumax">
                <div>
                  <xsl:if test= "$left = 'true'">
                    <xsl:attribute name="class">left_club_bottom</xsl:attribute>
                  </xsl:if>
                  <xsl:if test= "$left = 'false'">
                    <xsl:attribute name="class">right_club_bottom</xsl:attribute>
                  </xsl:if>

                  <span>
                    (<xsl:value-of select="//club[@ID=//participants/participant[@judoka=$participant2]/descendant::*[1]/@club]/nomCourt"/>)
                  </span>
                </div>
              </xsl:if>
            </xsl:if>-->
					</td>
				</tr>
				<xsl:if test= "$last = 'true'">
					<tr></tr>
				</xsl:if>
			</table>
		</div>
	</xsl:template>

	<xsl:template name="combatVainqueur">
		<xsl:param name="combat"/>

		<xsl:variable name="participant1" select="$combat/score[@judoka = $combat/@vainqueur ]/@judoka"/>


		<div class="alert alert-success">
			<xsl:if test= "//participants/participant[@judoka=$participant1]/descendant::*[1]/@nom">
				<xsl:attribute name="rowspan">2</xsl:attribute>
			</xsl:if>
			<xsl:if test= "not(//participants/participant[@judoka=$participant1]/descendant::*[1]/@nom)">
				<xsl:attribute name="style">border-bottom:1px solid #FFF;</xsl:attribute>
			</xsl:if>
			<xsl:if test= "//participants/participant[@judoka=$participant1]/descendant::*[1]/@nom">

				<xsl:value-of select="//participants/participant[@judoka=$participant1]/descendant::*[1]/@nom"/>
				<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
				<xsl:value-of select="//participants/participant[@judoka=$participant1]/descendant::*[1]/@prenom"/>

			</xsl:if>
		</div>

	</xsl:template>

	<xsl:template name="score">
		<xsl:param name="combat"/>
		<div class="score">
			<span>
				<xsl:value-of select="substring($combat/@scorevainqueur,0,3)"/>
			</span>
			<xsl:if test="$typeCompetition != '1'">
				<span style="color:red;">
					<xsl:value-of select="$combat/@penvainqueur"/>
				</span>
			</xsl:if>
			<xsl:if test="$combat/@scorevainqueur != ''">
				<span>
					<xsl:text disable-output-escaping="yes"> / </xsl:text>
				</span>
			</xsl:if>
			<span>
				<xsl:value-of select="substring($combat/@scoreperdant,0,3)"/>
			</span>
			<xsl:if test="$typeCompetition != '1'">
				<span style="color:red;">
					<xsl:value-of select="$combat/@penperdant"/>
				</span>
			</xsl:if>
		</div>
	</xsl:template>

	<xsl:template name="power">
		<xsl:param name="base"/>
		<xsl:param name="power"/>

		<xsl:variable name="powerTMP">
			<xsl:choose>
				<xsl:when test="power &lt; 0">
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
					<xsl:call-template name="power">
						<xsl:with-param name="base" select="$base"/>
						<xsl:with-param name="power" select="$powerTMP - 1"/>
					</xsl:call-template>
				</xsl:variable>
				<xsl:value-of select="$base * $temp"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>