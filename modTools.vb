Module modTools
    Property tablename As String = ""
    'Public Property area As Double

    Property enc As Text.Encoding
    Friend Property vid As String
    'Property rid As String
    'Property serial As String
    Private Property modus As String
    Private Property outfile As String
    Property sachgebiet As String

    'Property fs As String
    'Property gemcode As String
    'Property FsPositionInShapeFile As String = "1"
    Private Property username As String
    Private Property userEbeneAid As Integer
    Property postgis As String
    Public host, datenbank, schema, tabelle, dbuser, dbpw, dbport As String
    Dim erfolg As Boolean = False

    '----------------
    'Public Property isDebugmode As Boolean = True
    Dim paradigmaXML As String
    Public Sub getTablename(lokmodus As String, userEbeneAid As Integer)
        'l("#############username: " & username)
        'l("vid: " & vid)
        'l("modus: " & modus)
        If lokmodus = "einzeln" Then
            tablename = "tab" & CType(userEbeneAid, String) ' username '.Replace("-", "_")
        End If
        If lokmodus = "liste" Then tablename = outfile
        If lokmodus.ToLower.StartsWith("sachgebiet") Then tablename = "SG_" & modSachgebiet.getsachgebiet(lokmodus)
    End Sub

    'Friend Sub main3(username As String, vid As String, modus As String, outfile As String, isDebugmode As Boolean)
    '    Throw New NotImplementedException()
    'End Sub

    Sub createHeaderFile(layerfile As String, headerfile As String)
        ' /fkat/flurkarte/flurkarte2016/flurkarte2016_header.map"
        'MAP
        'INCLUDE '/inetpub/wwwroot/buergergis/mapfile/header.map',
        'INCLUDE '/fkat/boden/bodentyp/bodentyp_layer.map',,
        'End
        l("in createMapfilePDF--------------------------")
        Try
            Dim sb As New Text.StringBuilder
            sb.AppendLine("MAP")
            sb.AppendLine("INCLUDE '/inetpub/wwwroot/buergergis/mapfile/header.map'")
            sb.AppendLine("INCLUDE '" & layerfile & "'")
            sb.AppendLine("END")
            My.Computer.FileSystem.WriteAllText(headerfile, sb.ToString, False, enc)
            sb = Nothing
        Catch ex As Exception
            l("fehler in createMapfilePDF " & ex.ToString)
        End Try
    End Sub

    Public Function webgisPreparieren(ByVal mitetikett As Boolean, tablename As String) As String
        Try
            glob2.nachricht("webgisPreparieren------------------------------------")
            'ebene in webgiscontrol anlegen
            Dim KartenEbenenName As String = modMapserverlayer.kartenebenenName
            glob2.nachricht("vor makeMapFile")
            makeMapFilePostgis(modMapserverlayer.KartenMapfileTemplate, KartenMAPfile, KartenEbenenName,
            mitetikett, myglobalz.enc, tablename, userlayeraidNKATDIR)
            glob2.nachricht("nach makeMapFile")
            glob2.nachricht("headermapfile generieren")
            glob2.nachricht("outfile: " & userlayeraidNKATDIR & "header.map")
            createHeaderFile("/nkat/aid/" & userEbeneAid & "/layer.map", userlayeraidNKATDIR & "header.map")

            makeDBTemplateFilePostgis(modMapserverlayer.KartenMapfileTemplate, KartenMAPfile, KartenEbenenName,
                                     mitetikett, myglobalz.enc, tablename, userlayeraidNKATDIR)

            Dim ZielHTMfile As String = KartenMAPfile.Replace(".map", ".htm").Replace("d:",
                                                                                   myglobalz.gis_serverD)
            'makeHtmFile(KartenHTMfileTemplate, ZielHTMfile)
            glob2.nachricht("ZielHTMfile " & ZielHTMfile)
            glob2.nachricht("nach makeHtmFile")

            Dim directory As String = KartenMAPfile.Replace(myglobalz.gis_serverD$, "d:")
            Dim dbpfad$ = "" ' kartenDatadir$.Replace(myGlobalz.gis_serverD$, "")

            'DB_fork.insertFeatureClassIntoWebgiscontrolDB_alledb(KartenEbenenName, appendix$, directory, dbpfad, myGlobalz.haloREC)

            glob2.nachricht("webgisPreparieren ################ endee #")
            Return KartenEbenenName
        Catch ex As Exception
            l("protokoll now: " & ex.ToString)
            Return ""
        End Try
    End Function
    Function main2(_username As String, _vid As String, _modus As String,
                   ByRef returnstring As String, dbtyp As String, mac As String) As String
        l("main2 ----------------------------------- " & mac)
        username = _username
        vid = _vid
        modus = _modus
        'isDebugmode = _isDebugmode
#If DEBUG Then
        'paradigmaXML = "C:\acheckouts\paradigma\userlayer2Postgis\bin\Release\paradigma.xml" : l(paradigmaXML)
        paradigmaXML = "C:\auscheck2\userlayer2Postgis\bin\Release\paradigma.xml" : l(paradigmaXML)
        paradigmaXML = "C:\a_vs\NEUPara\userlayer2Postgis\bin\Debug\paradigma.xml" : l(paradigmaXML)
        paradigmaXML = "J:\test\paradigmaArchiv\div\xml\paradigma_2017.xml" : l(paradigmaXML)
        paradigmaXML = "J:\test\paradigmaArchiv\div\xml\paradigma_sqls.xml" : l(paradigmaXML)
        paradigmaXML = "l:\inetpub\scripts\apps\paradigmaex\layer2shpfile\userlayer2postgis\paradigma_sqls.xml"
#Else
        paradigmaXML = "xmlparadigma_2017.xml" : l(paradigmaXML)
        paradigmaXML = "d:\inetpub\scripts\apps\paradigmaex\layer2shpfile\userlayer2postgis\paradigma_sqlsO.xml"

#End If
        l("paradigmaXML " & paradigmaXML)

        myglobalz.gis_serverD = "d:"
        myglobalz.GIS_WebServer = "w2gis02.kreis-Of.local" '"KIS" 

        myglobalz.iniDict = clsINIXML.XMLiniReader(paradigmaXML) '"g:\appsconfig\para
        l("nachXMLiniReader inicount " & myglobalz.iniDict.Count)
        modMapserverlayer.ini_WebgisREC()
        modMapserverlayer.ini_raumbezug()

        l("username: " & username)
        l("vid: " & vid)
        l("modus: " & modus)
        l("outfile: " & outfile)
        l("a mac: " & mac)
        modPG.ini_PGREC(tablename)
        Dim altmac As String = ""
        Dim useridINtern As Integer
        userEbeneAid = modUserLayer.getUserebeneAid(username, useridINtern, altmac)
        l("userEbeneAid " & userEbeneAid)
        getTablename(_modus, userEbeneAid) : l("tablename: " & tablename)
        Postgis_MYDB.Tabelle = tablename
        If userEbeneAid < 1 Then
            userEbeneAid = modUserLayer.userLayerErzeugen(tablename, vid, _modus)
            If userEbeneAid = 0 Then
                'ebene lies sich nicht erzeugen !!! kommt vor
            Else
                getTablename(_modus, userEbeneAid) : l("tablename: " & tablename)
                If useridINtern < 1 Then
                    l("user hat nioch keine id - insert in nutzer")
                    'insert
                    l(useridINtern & " insert : " & userEbeneAid)
                    l(useridINtern & " mac : " & mac)
                    mac = mac.Replace(",", "")
                    modUserLayer.InsertInNutzertab(username, userEbeneAid, mac)
                Else
                    l("user hat schon eine id update nutzer")
                    'update
                    l(useridINtern & " update : " & userEbeneAid)
                    modUserLayer.updateNutzerTab(useridINtern, userEbeneAid, mac)
                End If
            End If
        Else
            l("userlayer ist schon vorhanden")
            'weiter
        End If

        modMapserverlayer.VerzeichnisseEinrichten(tablename, userEbeneAid)
        modMapserverlayer.Verzeichnisse_ausgeben()
        modMapserverlayer.Verzeichnisse_anlegen()
        Dim mitetikett As Boolean = False
        Dim KartenEbenenName As String = ""
        'modEinzeln.Testeo()
        If _modus = "einzeln" Then
            modMapserverlayer.kartenebenenName = CStr(_username)
            Postgis_MYDB.Tabelle = tablename
            modEinzeln.exekuteEinzelVorgang(CInt(_vid), aktbox, dbtyp)
            KartenEbenenName = webgisPreparieren(mitetikett, tablename)
            glob2.nachricht("KartenEbenenName$: " & KartenEbenenName)
            Return CStr(1)
        End If

        If _modus.ToLower.StartsWith("sachgebiet") Then
            l("entering sachgebiet -----------------------------")
            modMapserverlayer.kartenebenenName = tablename
            KartenMapfileTemplate = myglobalz.gis_serverD & "/paradigmacache/vorlagen/raumbezugIllegbau.map"
            KartenHTMfileTemplate = myglobalz.gis_serverD & "/paradigmacache/vorlagen/raumbezugIllegbau.htm"
            sachgebiet = modSachgebiet.getsachgebiet(_modus)

            modSachgebiet.exekuteSachgebiet(sachgebiet, aktbox, returnstring)
            KartenEbenenName = webgisPreparieren(mitetikett, tablename)
            glob2.nachricht("KartenEbenenName$: " & KartenEbenenName)
            Return CStr(1)
            l("fertig sachgebiet -----------------------------")
        End If
        If _modus = "liste" Then
            Dim anzahl As String
            modMapserverlayer.kartenebenenName = CStr(_outfile)
            anzahl = modListe.exekuteVorgangsListe(_outfile, aktbox)

            KartenEbenenName = webgisPreparieren(mitetikett, _outfile)
            glob2.nachricht("KartenEbenenName: " & KartenEbenenName)
            Return anzahl
        End If
        Return CStr(0)
    End Function
    Sub makeMapFilePostgis(ByVal inTemplateMapfile As String,
                    ByVal outKartenMAPfile As String,
                    ByVal KartenEbenenName As String,
                    ByVal mitetikett As Boolean,
                    enc As Text.Encoding,
                    tableName As String,
                                  userlayeraidNKATDIR As String)
        l("makeMapFile -----------------------------------------------")
        l(" templateMapfile$: " & inTemplateMapfile)
        l(" KartenMAPfile$$: " & outKartenMAPfile)
        Dim tempsafe, neusave As String
        Try
            If IO.File.Exists(inTemplateMapfile) Then
                l("Vorlage existiert")
                Using selVorlage As New IO.StreamReader(inTemplateMapfile, enc)
                    tempsafe = selVorlage.ReadToEnd
                    neusave = tempsafe
                    tempsafe = tempsafe.Replace("[FEATURECLASS]", KartenEbenenName)

                    tempsafe = tempsafe.Replace("[PG_SCHEMA.TABELLE]", "paradigma_userdata." & tableName.ToLower)
                    tempsafe = tempsafe.Replace("[TABLENAME]", tableName.Trim.ToLower)
                    If Not mitetikett Then
                        tempsafe = tempsafe.Replace("Labelitem 'RBTITEL'#beipoint", "Labelitem 'RBTYP'")
                    End If
                End Using
                My.Computer.FileSystem.WriteAllText(outKartenMAPfile, tempsafe, False, enc)

                neusave = tempsafe.Replace("Imagemapmaxscale", "#Imagemapmaxscale")
                neusave = neusave.Replace("Imagemap", "#Imagemap")
                neusave = neusave.Replace("#Template", " Template ")
                neusave = neusave.Replace("#Header", " Header ")
                neusave = neusave.Replace("#Footer", " Footer ")
                My.Computer.FileSystem.WriteAllText(userlayeraidNKATDIR & "layer.map", neusave, False, enc)

                l("Mapfile$ wurde erzeugt: " & KartenEbenenName)
            Else
                l("FEHLER: Vorlage exitiert nicht")
            End If
        Catch ex As Exception
            l("fehler in makeMapFilePostgis " & ex.ToString)
        End Try
    End Sub
    Friend Sub makeDBTemplateFilePostgis(inTemplateMapfile As String, outKartenMAPfile As String, kartenEbenenName As String,
                                            mitetikett As Boolean, enc As System.Text.Encoding, tableName As String,
                                            userlayeraidNKATDIR As String)
        l("makeDBTemplateFilePostgis -----------------------------------------------")
        l(" templateMapfile$: " & inTemplateMapfile)
        l(" KartenMAPfile$$: " & outKartenMAPfile)
        Dim tempsafe, neusave As String
        inTemplateMapfile = inTemplateMapfile.Replace("/", "\")
        outKartenMAPfile = outKartenMAPfile.Replace("/", "\")

        inTemplateMapfile = inTemplateMapfile.Replace("raumbezug.map", "raumbezug_templ.htm")
        outKartenMAPfile = userlayeraidNKATDIR & "raumbezug_templ.htm"
        ausschreiben(inTemplateMapfile, outKartenMAPfile)

        inTemplateMapfile = inTemplateMapfile.Replace("raumbezug_templ.htm", "raumbezug_circle_templ.htm")
        outKartenMAPfile = userlayeraidNKATDIR & "raumbezug_circle_templ.htm"
        ausschreiben(inTemplateMapfile, outKartenMAPfile)

        inTemplateMapfile = inTemplateMapfile.Replace("raumbezug_circle_templ.htm", "raumbezug_line_templ.htm")
        outKartenMAPfile = userlayeraidNKATDIR & "raumbezug_line_templ.htm"
        ausschreiben(inTemplateMapfile, outKartenMAPfile)
    End Sub

    Private Sub ausschreiben(inTemplateMapfile As String, outKartenMAPfile As String)
        Try
            Dim fi As New IO.FileInfo(inTemplateMapfile)
            If fi.Exists Then
                l("Vorlage existiert")
                fi.CopyTo(outKartenMAPfile, True)
                l(outKartenMAPfile & "  erzeugt")

            Else
                l("FEHLER: Vorlage exitiert nicht")
            End If
        Catch ex As Exception
            l("fehler in makeDBTemplateFilePostgis " & ex.ToString)
        End Try
    End Sub
End Module
