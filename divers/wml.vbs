	dim xmlDom
        dim xslDom
        dim res
        Dim fso
        Dim ftxt

        Set xmlDom = CreateObject("MSXML2.DOMDocument")
        Set xslDom = CreateObject("MSXML2.DOMDocument")
        Set fso    = CreateObject("Scripting.FileSystemObject")

        xmlDom.load "wml.wml"
        xslDom.load "wml.xsl"

        res = xmlDom.transformNode(xslDom)

        Set ftxt = fso.CreateTextFile("wml.html", True)
        ftxt.Write res
        ftxt.Close

