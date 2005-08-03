	dim xmlDom
        dim xslDom
        dim res
        Dim fso
        Dim ftxt

        Set xmlDom = CreateObject("MSXML2.DOMDocument")
        Set xslDom = CreateObject("MSXML2.DOMDocument")
        Set fso    = CreateObject("Scripting.FileSystemObject")

        xmlDom.load "recents.xml"
        xslDom.load "recents.xsl"

        res = xmlDom.transformNode(xslDom)

        Set ftxt = fso.CreateTextFile("recents.html", True)
        ftxt.Write res
        ftxt.Close

