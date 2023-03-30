<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" indent="yes" />
	<xsl:param name="style"></xsl:param>
	<xsl:param name="rep"></xsl:param>

	<xsl:key name="combats" match="combat" use="@niveau"/>


	<xsl:variable name="repechage">
		<xsl:text>false</xsl:text>
	</xsl:variable>
	<xsl:variable name="typeCompetition" select="/competition/@type">
	</xsl:variable>

	<xsl:variable name="IsIntegral">
		<xsl:choose>
			<xsl:when test="//phase[1]/@niveauRepechage = 2 and //phase[1]/@niveauRepeches = 3">
				<xsl:text>true</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>false</xsl:text>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="IsPhaseValide">
		<xsl:choose>
			<xsl:when test="//phase[1]/@etat = 5">
				<xsl:text>true</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>false</xsl:text>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:template match="/">
		<xsl:variable name="niveaumax">
			<xsl:for-each select="//combat[@repechage=$repechage and generate-id() = generate-id(key('combats',@niveau)[1])]">
				<xsl:sort select="@niveau" data-type="number" order="descending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:variable name="niveaumin">
			<xsl:for-each select="//combat[generate-id() = generate-id(key('combats',@niveau)[1])]">
				<xsl:sort select="@niveau" data-type="number" order="ascending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
				<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

				<link rel="stylesheet" type="text/css" >
					<xsl:attribute name="href">
						file:///<xsl:value-of select="$style"/>style_feuille_tableau_8.css
					</xsl:attribute>
				</link>

				<title>-60</title>
			</head>
			<body>
				<div class="main">
					<div class="epreuve_c">
						<table>
							<tr>
								<td style="font-size: 1.3em;">
									<xsl:if test="$typeCompetition != '1'">
										<xsl:value-of select="//epreuve[1]/@nom_cateage"/>
										<xsl:text>&#32;</xsl:text>
										<xsl:value-of select="//epreuve[1]/@sexe"/>
										<xsl:text>&#32;</xsl:text>
									</xsl:if>
								</td>
							</tr>
							<tr>
								<xsl:if test="$typeCompetition != '1'">
									<td style="font-size: 2.7em;">
										<xsl:if test="not(contains(//epreuve[1]/@nom, '+'))">
											<xsl:text disable-output-escaping="yes">-</xsl:text>
											<xsl:value-of select="//epreuve[1]/@nom"/>
										</xsl:if>
										<xsl:if test="contains(//epreuve[1]/@nom, '+')">
											<xsl:value-of select="//epreuve[1]/@nom"/>
										</xsl:if>
									</td>
								</xsl:if>
								<xsl:if test="$typeCompetition = '1'">
									<td style="font-size: 2.5em;">
										<xsl:value-of select="//epreuve[1]/@nom"/>
									</td>
								</xsl:if>
							</tr>
							<tr>
								<td style="font-size: 1.3em;">
									<xsl:if test="//phase[1]/@isequipe='false' ">
										<xsl:value-of select="count(//participant)"/>
										<xsl:text disable-output-escaping="yes">&#32;Judokas</xsl:text>
									</xsl:if>
									<xsl:if test="//phase[1]/@isequipe='true' ">
										<xsl:value-of select="count(//participant)"/>
										<xsl:text disable-output-escaping="yes">&#32;Equipes</xsl:text>
									</xsl:if>
								</td>
							</tr>
						</table>
					</div>
					<div class="principale">
						<table class="t1" style="page-break-inside: avoid; width:100%">

							<tr>
								<td rowspan="1" class="td_texte1">
									<xsl:for-each select="key('combats','3')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 1">
											<xsl:call-template name="participant1">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>1</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>

								<td rowspan="2" class="td_tab">
									<div class="line">
										<table>
											<tr>
												<td />
											</tr>
											<tr>
												<td class="line1" rowspan="2">
													<div class="arrondi" />
												</td>
												<td class="line2" />
											</tr>
											<tr>
												<td />
											</tr>
											<tr>
												<td />
											</tr>
										</table>
									</div>
								</td>
								<td rowspan="2" class="td_texte2">
									<xsl:for-each select="key('combats','2')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 1">
											<xsl:call-template name="participant2">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>1</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>

								<td rowspan="4" class="td_tab">
									<div class="line">
										<table>
											<tr>
												<td />
											</tr>
											<tr>
												<td class="line1" rowspan="2">
													<div class="arrondi" />
												</td>
												<td class="line2" />
											</tr>
											<tr>
												<td />
											</tr>
											<tr>
												<td />
											</tr>
										</table>
									</div>
								</td>
								<td rowspan="4" class="td_texte2">
									<xsl:for-each select="key('combats','1')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 1">
											<xsl:call-template name="participant2">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>1</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>

								<td rowspan="9" class="td_tab">
									<div class="line">
										<table>
											<tr style="height: 16%;">
												<td />
											</tr>
											<tr style="height: 16%;">
												<td class="line1" rowspan="4">
													<div class="arrondi2" />
												</td>
												<td class="line2" />
											</tr>
											<tr style="height: 16%;">
												<td />
											</tr>
											<tr style="height: 16%;">
												<td />
											</tr>
											<tr style="height: 16%;">
												<td />
											</tr>
											<tr style="height: 16%;">
												<td />
											</tr>
										</table>
									</div>
								</td>

							</tr>
							<tr>
								<td rowspan="1">
									<xsl:for-each select="key('combats','3')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 1">
											<xsl:call-template name="participant1">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>2</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>
							</tr>

							<tr>
								<td rowspan="1" class="td_texte1">
									<xsl:for-each select="key('combats','3')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 2">
											<xsl:call-template name="participant1">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>1</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>

								<td rowspan="2" class="td_tab">
									<div class="line">
										<table>
											<tr>
												<td />
											</tr>
											<tr>
												<td class="line1" rowspan="2">
													<div class="arrondi" />
												</td>
												<td class="line2" />
											</tr>
											<tr>
												<td />
											</tr>
											<tr>
												<td />
											</tr>
										</table>
									</div>
								</td>
								<td rowspan="2" class="td_texte2">
									<xsl:for-each select="key('combats','2')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 1">
											<xsl:call-template name="participant2">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>2</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>

							</tr>
							<tr>
								<td rowspan="1">
									<xsl:for-each select="key('combats','3')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 2">
											<xsl:call-template name="participant1">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>2</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>
							</tr>


							<tr>
								<td></td>
								<td></td>

								<td></td>
								<td></td>


								<td>
									<xsl:for-each select="key('combats','1')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 1">
											<xsl:call-template name="vainqueur">
												<xsl:with-param name="combat" select="."/>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>
							</tr>


							<tr>
								<td rowspan="1" class="td_texte1">
									<xsl:for-each select="key('combats','3')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 3">
											<xsl:call-template name="participant1">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>1</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>

								<td rowspan="2" class="td_tab">
									<div class="line">
										<table>
											<tr>
												<td />
											</tr>
											<tr>
												<td class="line1" rowspan="2">
													<div class="arrondi" />
												</td>
												<td class="line2" />
											</tr>
											<tr>
												<td />
											</tr>
											<tr>
												<td />
											</tr>
										</table>
									</div>
								</td>
								<td rowspan="2" class="td_texte2">
									<xsl:for-each select="key('combats','2')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 2">
											<xsl:call-template name="participant2">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>1</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>

								<td rowspan="4" class="td_tab">
									<div class="line">
										<table>
											<tr>
												<td />
											</tr>
											<tr>
												<td class="line1" rowspan="2">
													<div class="arrondi" />
												</td>
												<td class="line2" />
											</tr>
											<tr>
												<td />
											</tr>
											<tr>
												<td />
											</tr>
										</table>
									</div>
								</td>
								<td rowspan="4" class="td_texte2">
									<xsl:for-each select="key('combats','1')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 1">
											<xsl:call-template name="participant2">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>2</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>

							</tr>
							<tr>
								<td rowspan="1">
									<xsl:for-each select="key('combats','3')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 3">
											<xsl:call-template name="participant1">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>2</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>
							</tr>

							<tr>

								<td rowspan="1" class="td_texte1">
									<xsl:for-each select="key('combats','3')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 4">
											<xsl:call-template name="participant1">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>1</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>

								<td rowspan="2" class="td_tab">
									<div class="line">
										<table>
											<tr>
												<td />
											</tr>
											<tr>
												<td class="line1" rowspan="2">
													<div class="arrondi" />
												</td>
												<td class="line2" />
											</tr>
											<tr>
												<td />
											</tr>
											<tr>
												<td />
											</tr>
										</table>
									</div>
								</td>
								<td rowspan="2" class="td_texte2">
									<xsl:for-each select="key('combats','2')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 2">
											<xsl:call-template name="participant2">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>2</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>

							</tr>
							<tr>
								<td rowspan="1">
									<xsl:for-each select="key('combats','3')">
										<xsl:sort select="@reference" order="ascending"/>
										<xsl:if test="position() = 4">
											<xsl:call-template name="participant1">
												<xsl:with-param name="combat" select="."/>
												<xsl:with-param name="judoka">
													<xsl:text>2</xsl:text>
												</xsl:with-param>
											</xsl:call-template>
										</xsl:if>
									</xsl:for-each>
								</td>
							</tr>

						</table>
					</div>

					<div class="sp_line">
						<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
					</div>

					<xsl:if test="//phase[1]/@etat = 5">
						<div class="classement">
							<div style="position:absolute;width:100%">
								<div class="classement_title">
									<div class="classement_title_div">CLASSEMENT</div>
								</div>
							</div>
							<table>
								<xsl:for-each select="//phase/participants/participant">
									<xsl:sort select="@classementFinal" data-type="number" order="ascending"/>

									<xsl:variable name="j1" select="descendant::*[1]" />

									<xsl:if test="@classementFinal != 0 and @classementFinal &lt; 9">
										<tr>
											<td class="align_center">
												<xsl:value-of select="@classementFinal"/>
												<xsl:text disable-output-escaping="yes">.</xsl:text>
											</td>
											<td>
												<xsl:if test="$typeCompetition = '1'">
													<xsl:if test="$j1/judoka/@serie != '0'">
														<span style="font-weight: 600;">
															<xsl:value-of select="./equipe/@nom"/>
														</span>
													</xsl:if>
													<xsl:if test="$j1/judoka/@serie = '0'">
														<xsl:value-of select="./equipe/@nom"/>
													</xsl:if>
												</xsl:if>
												<xsl:if test="$typeCompetition != '1'">
													<xsl:if test="$j1/@serie != '0'">
														<span style="font-weight: 600;">
															<xsl:value-of select="./judoka/@nom"/>
															<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
															<xsl:value-of select="./judoka/@prenom"/>
														</span>
													</xsl:if>
													<xsl:if test="$j1/@serie = '0'">
														<xsl:value-of select="./judoka/@nom"/>
														<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
														<xsl:value-of select="./judoka/@prenom"/>
													</xsl:if>
												</xsl:if>

											</td>
											<td>
												<xsl:variable name="club" select="$j1/@club"/>
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

												<xsl:variable name="secteur" select="//comite[@ID=$comite]/@secteur"/>

												<xsl:variable name="ecartement" select="ancestor::phase[1]/@ecartement"/>
												<xsl:choose>
													<xsl:when test= "$ecartement = '3'">
														<xsl:if test="//club[@ID=$club]/nomCourt">
															<xsl:value-of select="//club[@ID=$club]/nomCourt"/>
															<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														</xsl:if>
														<xsl:value-of select="$comite"/>
													</xsl:when>

													<xsl:when test= "$ecartement = '4'">
														<xsl:if test="//club[@ID=$club]/nomCourt">
															<xsl:value-of select="//club[@ID=$club]/nomCourt"/>
															<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														</xsl:if>
														<xsl:value-of select="//ligue[@ID=$ligue]/nomCourt"/>
													</xsl:when>

													<xsl:when test= "$ecartement = '9'">
														<xsl:if test="//club[@ID=$club]/nomCourt">
															<xsl:value-of select="//club[@ID=$club]/nomCourt"/>
															<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														</xsl:if>
														<xsl:value-of select="//secteur[@ID=$secteur]/nomCourt"/>
													</xsl:when>

													<xsl:otherwise>
														<xsl:value-of select="//club[@ID=$club]/nomCourt"/>
													</xsl:otherwise>
												</xsl:choose>
											</td>
										</tr>
									</xsl:if>

								</xsl:for-each>
							</table>
						</div>
					</xsl:if>
				</div>

				<xsl:variable name="file" select="document($rep,.)"/>

				<xsl:if test="count($file//table) &gt; 0">
					<div class="repechage">

						<xsl:for-each select="$file//table">
							<xsl:variable name="position_table" select="position()" />
							<div>
								<xsl:if test="$position_table = 1">
									<xsl:attribute name="class">repechage1</xsl:attribute>
								</xsl:if>
								<xsl:if test="$position_table = 2">
									<xsl:attribute name="class">repechage2</xsl:attribute>
								</xsl:if>
								<table class="t1" style="page-break-inside: avoid;">

									<xsl:for-each select="./tr">
										<xsl:variable name="position_tr" select="position()" />
										<tr>
											<xsl:for-each select="./td">
												<xsl:variable name="position_td" select="position()" />
												<td>
													<xsl:attribute name="rowspan">
														<xsl:value-of select="./@rowspan"/>
													</xsl:attribute>

													<xsl:variable name="type">
														<xsl:value-of select="./@type"/>
													</xsl:variable>
													<xsl:choose>
														<xsl:when test="$type = 1">
															<xsl:attribute name="class">td_texte3</xsl:attribute>
															<xsl:choose>
																<xsl:when test= "./combat">


																	<xsl:variable name="combat1">
																		<xsl:value-of select="./combat/@reference"/>
																	</xsl:variable>
																	<xsl:variable name="judoka1">
																		<xsl:value-of select="./combat/@judoka"/>
																	</xsl:variable>

																	<xsl:variable name="combat2" select="//combat[@reference=$combat1]">
																	</xsl:variable>

																	<xsl:call-template name="participant3">
																		<xsl:with-param name="combat" select="$combat2"/>
																		<xsl:with-param name="judoka" select="$judoka1"/>
																	</xsl:call-template>

																</xsl:when>
																<xsl:otherwise>
																	<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
																</xsl:otherwise>
															</xsl:choose>
														</xsl:when>

														<xsl:when test="$type = 2">
															<xsl:attribute name="class">td_tab</xsl:attribute>
															<div class="line">
																<table>
																	<tr>
																		<td />
																	</tr>
																	<tr>
																		<td class="line1" rowspan="2">
																			<div class="arrondi" />
																		</td>
																		<td class="line2" />
																	</tr>
																	<tr>
																		<td />
																	</tr>
																	<tr>
																		<td />
																	</tr>
																</table>
															</div>
														</xsl:when>

														<xsl:when test="$type = 3">
															<xsl:attribute name="class">td_tab</xsl:attribute>
															<div class="line">
																<table>
																	<tr style="height: 16%;">
																		<td />
																	</tr>
																	<tr style="height: 16%;">
																		<td />
																	</tr>
																	<tr style="height: 16%;">
																		<td class="line1" rowspan="4">
																			<div class="arrondi2" />
																		</td>
																		<td class="line2" />
																	</tr>

																	<tr style="height: 16%;">
																		<td />
																	</tr>
																	<tr style="height: 16%;">
																		<td />
																	</tr>
																	<tr style="height: 16%;">
																		<td />
																	</tr>
																</table>
															</div>
														</xsl:when>

														<xsl:when test="$type = 4">
															<xsl:attribute name="class">td_texte3</xsl:attribute>

															<xsl:variable name="combat1">
																<xsl:value-of select="./combat/@reference"/>
															</xsl:variable>
															<xsl:variable name="judoka1">
																<xsl:value-of select="./combat/@judoka"/>
															</xsl:variable>

															<xsl:variable name="combat2" select="//combat[@reference=$combat1]">
															</xsl:variable>

															<xsl:call-template name="vainqueur">
																<xsl:with-param name="combat" select="$combat2"/>
															</xsl:call-template>

														</xsl:when>

														<xsl:when test="$type = 5">
															<xsl:attribute name="class">td_tab</xsl:attribute>
															<div class="line">
																<table>
																	<tr>
																		<td />
																	</tr>
																	<tr>
																		<td class="line1" rowspan="3">
																			<div class="arrondi" />
																		</td>
																		<td class="line2" />
																	</tr>
																	<tr>
																		<td />
																	</tr>
																	<tr>
																		<td />
																	</tr>
																</table>
															</div>
														</xsl:when>

													</xsl:choose>
													<xsl:value-of select="."/>
												</td>
											</xsl:for-each>
										</tr>
									</xsl:for-each>
								</table>
							</div>
						</xsl:for-each>

						<!--TABLEAU REPECHAGE-->
						<!--
            <xsl:variable name="repechage1">
              <xsl:text>true</xsl:text>
            </xsl:variable>

            <xsl:call-template name="tableau">
              <xsl:with-param name="repechage" select="$repechage1">
              </xsl:with-param>
            </xsl:call-template>-->

					</div>
				</xsl:if>
			</body>
		</html>
	</xsl:template>


	<xsl:template name="participant1">
		<xsl:param name="combat"/>
		<xsl:param name="judoka"/>


		<xsl:variable name="participant1">
			<xsl:if test="$judoka = 1">
				<xsl:value-of select="$combat/score[1]/@judoka"/>
			</xsl:if>
			<xsl:if test="$judoka = 2">
				<xsl:value-of select="$combat/score[2]/@judoka"/>
			</xsl:if>
		</xsl:variable>

		<xsl:variable name="j1" select="//participants/participant[@judoka=$participant1]/descendant::*[1]" />


		<div>
			<xsl:if test="$IsPhaseValide = 'false' and $judoka = 1">
				<xsl:attribute name="class">ard1_1</xsl:attribute>
			</xsl:if>
			<xsl:if test="$IsPhaseValide = 'false' and $judoka = 2">
				<xsl:attribute name="class">ard2_1</xsl:attribute>
			</xsl:if>
			<xsl:if test="$IsPhaseValide = 'true' and $judoka = 1">
				<xsl:attribute name="class">ard1_2</xsl:attribute>
			</xsl:if>
			<xsl:if test="$IsPhaseValide = 'true' and $judoka = 2">
				<xsl:attribute name="class">ard2_2</xsl:attribute>
			</xsl:if>
			<div>
				<xsl:if test="$IsIntegral = 'true' and $IsPhaseValide = 'false'">
					<div class="col1">
						<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
						<xsl:value-of select="$combat/@numero"/>
						<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
					</div>
				</xsl:if>
			  <div>
				<xsl:if test="$typeCompetition = '1'">
					<xsl:choose>
						<xsl:when test="$j1/judoka/@serie != '0'">
							<xsl:attribute name="class">blocEq</xsl:attribute>
							<xsl:value-of select="$j1/judoka/@serie"/>
						</xsl:when>
						<xsl:otherwise>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
				<xsl:if  test="$typeCompetition != '1'">
					<xsl:attribute name="class">col1</xsl:attribute>
					<xsl:choose>
						<xsl:when test= "//ceinture[@id=$j1/@grade]/@nom != ''">
							<xsl:value-of select="//ceinture[@id=$j1/@grade]/@nom"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
			  </div>
			</div>

			<div class="col3">
				<xsl:choose>
					<xsl:when test= "$j1/@nom">
						<xsl:variable name="club" select="$j1/@club"/>
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

						<xsl:variable name="secteur" select="//comite[@ID=$comite]/@secteur"/>

						<xsl:variable name="ecartement" select="ancestor::phase[1]/@ecartement"/>
						<xsl:choose>
							<xsl:when test= "$ecartement = '3'">
								<xsl:if test="//club[@ID=$club]/nomCourt">
									<xsl:if test="$typeCompetition != '1'">
										<xsl:value-of select="//club[@ID=$club]/nomCourt"/>
										<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
									</xsl:if>
								</xsl:if>
								<xsl:value-of select="$comite"/>
							</xsl:when>

							<xsl:when test= "$ecartement = '4'">
								<xsl:if test="//club[@ID=$club]/nomCourt">
									<xsl:if test="$typeCompetition != '1'">
										<xsl:value-of select="//club[@ID=$club]/nomCourt"/>
										<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
									</xsl:if>
								</xsl:if>
								<xsl:value-of select="//ligue[@ID=$ligue]/nomCourt"/>
							</xsl:when>

							<xsl:when test= "$ecartement = '9'">
								<xsl:if test="//club[@ID=$club]/nomCourt">
									<xsl:if test="$typeCompetition != '1'">
										<xsl:value-of select="//club[@ID=$club]/nomCourt"/>
										<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
									</xsl:if>
								</xsl:if>
								<xsl:value-of select="//secteur[@ID=$secteur]/nomCourt"/>
							</xsl:when>

							<xsl:otherwise>
								<xsl:value-of select="//club[@ID=$club]/nomCourt"/>
							</xsl:otherwise>
						</xsl:choose>
						<!--(<xsl:value-of select="substring(//club[@ID=$j1/@club]/nomCourt, 1, 15)"/>)-->
					</xsl:when>
					<xsl:otherwise>
						<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</div>
			<div class="col2">
				<xsl:if test="$typeCompetition = '1'">
					<span style="font-size: 0.9em; font-weight:400;margin-right:10px;">
						<xsl:value-of select="$combat/@firstrencontrelib"/>
					</span>
				</xsl:if>

				<xsl:choose>
					<xsl:when test= "$j1/@nom">
						<xsl:value-of select="$j1/@nom"/>
						<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
						<xsl:value-of select="$j1/@prenom"/>
						<xsl:if test="$j1/@serie != '0'">
							<xsl:text disable-output-escaping="yes">&#032;(TS</xsl:text>
							<xsl:value-of select="$j1/@serie"/>
							<xsl:text disable-output-escaping="yes">)</xsl:text>
						</xsl:if>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text disable-output-escaping="yes">&#160;---------------------&#160; </xsl:text>
					</xsl:otherwise>
				</xsl:choose>

			</div>
		</div>
	</xsl:template>


	<xsl:template name="participant2">
		<xsl:param name="combat"/>
		<xsl:param name="judoka"/>


		<xsl:variable name="participant1">
			<xsl:if test="$judoka = 1">
				<xsl:value-of select="$combat/score[1]/@judoka"/>
			</xsl:if>
			<xsl:if test="$judoka = 2">
				<xsl:value-of select="$combat/score[2]/@judoka"/>
			</xsl:if>
		</xsl:variable>

		<xsl:variable name="j1" select="//participants/participant[@judoka=$participant1]/descendant::*[1]" />

		<xsl:variable name="ref">
			<xsl:if test="$judoka = 1">
				<xsl:value-of select="$combat/feuille/@ref1"/>
			</xsl:if>
			<xsl:if test="$judoka = 2">
				<xsl:value-of select="$combat/feuille/@ref2"/>
			</xsl:if>
		</xsl:variable>

		<xsl:variable name="combat_prec" select="//combat[@reference = $ref]">
		</xsl:variable>

		<div>

			<xsl:if test="$IsIntegral = 'true' and $IsPhaseValide = 'false'">
				<div class="col1">
					<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
					<xsl:value-of select="$combat/@numero"/>
					<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
				</div>
			</xsl:if>

			<div class="name">
				<xsl:if test="$typeCompetition != '1'">
					<xsl:choose>
						<xsl:when test= "$j1/@nom">
							<xsl:if test="$j1/@serie != '0'">
								<span style="font-weight: 600;">
									<xsl:value-of select="$j1/@nom"/>
									<xsl:if test="$j1/@prenom">
										<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
										<xsl:value-of select="substring($j1/@prenom,1,1)"/>
										<xsl:text disable-output-escaping="yes">.</xsl:text>
									</xsl:if>
								</span>
							</xsl:if>
							<xsl:if test="$j1/@serie = '0'">
								<xsl:value-of select="$j1/@nom"/>
								<xsl:if test="$j1/@prenom">
									<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
									<xsl:value-of select="substring($j1/@prenom,1,1)"/>
									<xsl:text disable-output-escaping="yes">.</xsl:text>
								</xsl:if>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>

				<xsl:if test="$typeCompetition = '1'">
					<xsl:choose>
						<xsl:when test= "$j1/@nom">
							<xsl:if test="$j1/judoka/@serie != '0'">
								<span style="font-weight: 600;">
									<xsl:value-of select="$j1/@nom"/>
									<xsl:if test="$j1/@prenom">
										<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
										<xsl:value-of select="substring($j1/@prenom,1,1)"/>
										<xsl:text disable-output-escaping="yes">.</xsl:text>
									</xsl:if>
								</span>
							</xsl:if>
							<xsl:if test="$j1/judoka/@serie = '0'">
								<xsl:value-of select="$j1/@nom"/>
								<xsl:if test="$j1/@prenom">
									<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
									<xsl:value-of select="substring($j1/@prenom,1,1)"/>
									<xsl:text disable-output-escaping="yes">.</xsl:text>
								</xsl:if>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
					<span style="font-size: 0.9em; font-weight:400;margin-left:10px;">
						<xsl:value-of select="$combat/@firstrencontrelib"/>
					</span>
				</xsl:if>
			</div>
			<div class="left_line">
				<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
			</div>

			<xsl:call-template name="score">
				<xsl:with-param name="combat" select="$combat_prec"/>
			</xsl:call-template>

		</div>
	</xsl:template>

	<xsl:template name="participant3">
		<xsl:param name="combat"/>
		<xsl:param name="judoka"/>

		<xsl:variable name="participant1">
			<xsl:if test="$judoka = 1">
				<xsl:value-of select="$combat/score[1]/@judoka"/>
			</xsl:if>
			<xsl:if test="$judoka = 2">
				<xsl:value-of select="$combat/score[2]/@judoka"/>
			</xsl:if>
		</xsl:variable>

		<xsl:variable name="j1" select="//participants/participant[@judoka=$participant1]/descendant::*[1]" />

		<xsl:variable name="ref">
			<xsl:if test="$judoka = 1">
				<xsl:value-of select="$combat/feuille/@ref1"/>
			</xsl:if>
			<xsl:if test="$judoka = 2">
				<xsl:value-of select="$combat/feuille/@ref2"/>
			</xsl:if>
		</xsl:variable>

		<xsl:variable name="combat_prec" select="//combat[@reference = $ref]">
		</xsl:variable>

		<div>
			<div class="name">

				<xsl:if test="$combat_prec/@repechage = 'false' and $IsIntegral = 'true' and $IsPhaseValide = 'false'">
					<div class="col1">
						<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
						<xsl:value-of select="$combat_prec/@numero"/>
						<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
					</div>
				</xsl:if>

				<xsl:if test="$typeCompetition = '1'">
					<xsl:choose>
						<xsl:when test="$j1/judoka/@serie != '0'">
							<xsl:if test="$combat/@niveau = '-1'">
								<span class="part3Eq">
									<xsl:value-of select="$j1/judoka/@serie"/>
								</span>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
						</xsl:otherwise>
					</xsl:choose>

					<xsl:choose>
						<xsl:when test= "$j1/@nom">
							<xsl:if test="$j1/judoka/@serie != '0'">
								<span style="font-weight: 600;">
									<xsl:value-of select="$j1/@nom"/>
									<xsl:if test="$j1/@prenom">
										<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
										<xsl:value-of select="substring($j1/@prenom,1,1)"/>
										<xsl:text disable-output-escaping="yes">.</xsl:text>
									</xsl:if>
								</span>
							</xsl:if>
							<xsl:if test="$j1/judoka/@serie = '0'">
								<xsl:value-of select="$j1/@nom"/>
								<xsl:if test="$j1/@prenom">
									<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
									<xsl:value-of select="substring($j1/@prenom,1,1)"/>
									<xsl:text disable-output-escaping="yes">.</xsl:text>
								</xsl:if>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
						</xsl:otherwise>
					</xsl:choose>

					<span style="font-size: 0.9em; font-weight:400;margin-left:10px;">
						<xsl:value-of select="$combat/@firstrencontrelib"/>
					</span>
				</xsl:if>

				<xsl:if test="$typeCompetition != '1'">
					<xsl:choose>
						<xsl:when test= "$j1/@nom">
							<xsl:if test="$j1/@serie != '0'">
								<span style="font-weight: 600;">
									<xsl:value-of select="$j1/@nom"/>
									<xsl:if test="$j1/@prenom">
										<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
										<xsl:value-of select="substring($j1/@prenom,1,1)"/>
										<xsl:text disable-output-escaping="yes">.</xsl:text>
									</xsl:if>
									<xsl:if test="$combat/@niveau = '-1'">
										<xsl:text disable-output-escaping="yes">&#032;(TS</xsl:text>
										<xsl:value-of select="$j1/@serie"/>
										<xsl:text disable-output-escaping="yes">)</xsl:text>
									</xsl:if>
								</span>
							</xsl:if>
							<xsl:if test="$j1/@serie = '0'">
								<xsl:value-of select="$j1/@nom"/>
								<xsl:if test="$j1/@prenom">
									<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
									<xsl:value-of select="substring($j1/@prenom,1,1)"/>
									<xsl:text disable-output-escaping="yes">.</xsl:text>
								</xsl:if>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>

			</div>
			<div class="left_line">
				<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
			</div>

			<xsl:if test= "$combat/@repechage = $combat_prec/@repechage">
				<xsl:call-template name="score">
					<xsl:with-param name="combat" select="$combat_prec"/>
				</xsl:call-template>
			</xsl:if>

		</div>
	</xsl:template>

	<xsl:template name="vainqueur">
		<xsl:param name="combat"/>
		<xsl:variable name="participant1">
			<xsl:choose>
				<xsl:when test= "$combat/score[1]/@judoka = $combat/@vainqueur">
					<xsl:value-of select="$combat/score[1]/@judoka"/>
				</xsl:when>
				<xsl:when test= "$combat/score[2]/@judoka = $combat/@vainqueur">
					<xsl:value-of select="$combat/score[2]/@judoka"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text disable-output-escaping="yes">0</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="j1" select="//participants/participant[@judoka=$participant1]/descendant::*[1]" />

		<div class="vainqueur_name">
			<div>
				<xsl:if test="$typeCompetition != '1'">
					<xsl:choose>
						<xsl:when test= "$j1/@nom">
							<xsl:if test="$j1/@serie != '0'">
								<span style="font-weight: 600;">
									<xsl:value-of select="$j1/@nom"/>
									<xsl:if test="$j1/@prenom">
										<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
										<xsl:value-of select="substring($j1/@prenom,1,1)"/>
										<xsl:text disable-output-escaping="yes">.</xsl:text>
									</xsl:if>
								</span>
							</xsl:if>
							<xsl:if test="$j1/@serie = '0'">
								<xsl:value-of select="$j1/@nom"/>
								<xsl:if test="$j1/@prenom">
									<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
									<xsl:value-of select="substring($j1/@prenom,1,1)"/>
									<xsl:text disable-output-escaping="yes">.</xsl:text>
								</xsl:if>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
				<xsl:if test="$typeCompetition = '1'">
					<xsl:choose>
						<xsl:when test= "$j1/@nom">
							<xsl:if test="$j1/judoka/@serie != '0'">
								<span style="font-weight: 600;">
									<xsl:value-of select="$j1/@nom"/>
									<xsl:if test="$j1/@prenom">
										<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
										<xsl:value-of select="substring($j1/@prenom,1,1)"/>
										<xsl:text disable-output-escaping="yes">.</xsl:text>
									</xsl:if>
								</span>
							</xsl:if>
							<xsl:if test="$j1/judoka/@serie = '0'">
								<xsl:value-of select="$j1/@nom"/>
								<xsl:if test="$j1/@prenom">
									<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
									<xsl:value-of select="substring($j1/@prenom,1,1)"/>
									<xsl:text disable-output-escaping="yes">.</xsl:text>
								</xsl:if>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
			</div>

			<div class="left_line">
				<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
			</div>

			<xsl:call-template name="score">
				<xsl:with-param name="combat" select="$combat"/>
				<xsl:with-param name="scoreVainq">
					<xsl:text>1</xsl:text>
				</xsl:with-param>
			</xsl:call-template>

		</div>
	</xsl:template>

	<xsl:template name="score">
		<xsl:param name="combat"/>
		<xsl:param name="scoreVainq"/>

		<div>
			<xsl:if test="$scoreVainq = '1'">
				<xsl:attribute name="class">scoreVainq</xsl:attribute>
			</xsl:if>
			<xsl:if test="$scoreVainq != '1'">
				<xsl:attribute name="class">score</xsl:attribute>
			</xsl:if>

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