using System;

namespace GerarCargaDez.Telas.Etiqueta.Models
{
    class ModeloZebra
    {
        public static string ModeloEtiquetaNova(int Id, int Item_Id, string Item_Desc, string Lote_Serie, string Data_prod, string Data_val, int Seq, string Peso_liq, string Peso_bru, string Grupo_Ql, string hora_imp, bool update, bool caixa, string Obs)
        {
            string hora = update ? DateTime.Now.ToShortTimeString() : hora_imp;
            string useCX = caixa ? "UNIDADE" : "G.Q";
            string text =

            "^XA" +
            "^FO566,566^GFA,1300,1300,26,,::M038,M07F,M03FF,M03FFCgS07,M03IFY01FCQ0FC,M01IF83W03FCP01FC,M03KF7FFES03FCP03FE,M01KF7IFS03FCP03FE,M03KF7IF8R03F8P03FE,M01OFCR03F8P03FE,M01OFER03F8P01FC,M01OFER03F8Q0FC,M01JF8JFER07F8Q07,M03JF0FE3FEI0C00EJ087F8V0C,M03JF2FE1FF3F9E07F8007E7F8FF1FE3IFC3F87F3E,M03IFE7FE1FE3FFE0FFC01FF7F0FF1FE7IFC3F87F7C,M03IFC7FE1FE7FFE1FFE03JF0FE1FC7IF83F87FFC,M0JF8FFE3FE7FFC3IF03JF0FE3FC7IF87F87FFC,M0JF1KFE7FFC7IF07JF0FE3FC7IF07F8IFC,L01IFE3KFE7FFC7IF87JF1FE3FC7IF07F8IFC,L03IFC3KFC7FFCJF87JF1FE3FC7FFE07F8IF8,002I0JF87KFC7FCCFF7F8FFBFE1FE3FC03FC07F0FF88,003803JF0LF8FF80FE3F8FF1FE1FE3F803FC07F0FF,007LFE1LF0FF00FE3F8FF1FE1FE7F807F80FF0FF,003LFC3KFE0FF01FC3F8FE1FE1FC7F80FF00FF0FE,007LF87KF00FF01FE3F8FF1FE1FC7F80FF00FF1FE,007KFE0JF8I0FF00FE7F8FF3FE1FEFF81FE00FF1FE,00LFC1JF8I0FF00JF8JFE1JF03IF8FF1FE,00LF83JF8I0FE00JF0JFC1JF03IF8FF1FE,01LF07JF8001FE00JF0JFC1JF07IF9FE1FE,01KFC0KF8001FE007FFE0JFC1IFE07IF1FE1FE,03KF81KFI01FE007FFC07IFC0IFE0JF1FE1FC,03JFE03KFI01FE003FFC07IFC07FFC1JF1FE3FC,01JF807KFI01FE001FF003FBFC03FF01JF1FE3FC,01JF01JF8P07CI0FK0FE,00IFC03JF8,00FFE007JF,003F001JFE,L03JFC,L0KF8,K01KF,K07JFC,K03JF,L07FF8,M078,,:^FS" +
            "^FT545,637^A0N,14,19^FH\n^FDPROGRAMA DE DESENVOLVIMENTO^FS" +
            "^FT545,654^A0N,14,19^FH\n^FDINDUSTRIAL DE GOIAS^FS" +

            "^MMT" +
            "^PW831" +
            "^LL0679" +
            "^LS0" +
            "^FT512,640^XG000.GRF,1,1 ^FS" +
            "^FT56,72^A0N,62,79^FH\n^FD  DEZ ALIMENTOS  ^FS" +

            "^FT64,634^BY3,2,67^B7N,,Y,N^FD " + Item_Id + ";" + Lote_Serie + ";" + Data_prod + "; " + Data_val + ";" + Seq + ";" + Peso_liq + "; ^FS" +

            "^FO51,533^GB780,0,3^FS" +
            "^FO57,318^GB774,0,3^FS" +
            "^FO55,426^GB776,0,3^FS" +
            "^FO53,134^GB778,0,3^FS" +
            "^FO572,321^GB0,212,3^FS" +
            "^FO283,321^GB0,212,3^FS" +

            "^FT600,215^A0N,45,65^FH\\^FD  " + Grupo_Ql + " ^FS" +
            "^FT690,47^A0N,34,34^FH\\^FD   " + hora + "  ^FS" +
            "^FT710,105^A0N,34,34^FH\\^FD  " + Seq + "  ^FS" +
            "^FT54,215^A0N,48,72^FH\\^FD   " + Item_Id + " ^FS" +
            "^FT50,294^A0N,38,38^FH\\^FD   " + Item_Desc + " - " + Obs + "  ^FS" +
            "^FT554,413^A0N,48,48^FH\\^FD  " + Data_val + "  ^FS" +
            "^FT288,413^A0N,48,48^FH\\^FD  " + Data_prod + " ^FS" +
            "^FT15,414^A0N,40,40^FH\\^FD   " + Lote_Serie + "  ^FS" +
            "^FT56,521^A0N,48,48^FH\\^FD   " + Id + " ^FS" +
            "^FT340,523^A0N,48,48^FH\\^FD  " + Peso_liq + " ^FS" +
            "^FT582,521^A0N,48,48^FH\\^FD  " + Peso_bru + " ^FS" +

            "^FT690,51^A0N,21,20^FH\\^FDHr:^FS" +
            "^FT679,92^A0N,21,20^FH\\^FDNro:^FS" +
            "^FT372,101^A0N,17,19^FH\\^FDTel.: (0xx64)3413-8800^FS" +
            "^FT56,105^A0N,17,19^FH\\^FDCNPJ: 04.945.225/0001-71^FS" +
            "^FT56,126^A0N,17,19^FH\\^FDEndere\\87o: Rod GO 476 km13 S/N - Zona Rural - Morrinhos - GO ^FS" +
            "^FT609,165^A0N,21,20^FH\\^FD " + useCX + ":^FS" +
            "^FT54,165^A0N,20,19^FH\\^FDITEM:^FS" +
            "^FT579,348^A0N,23,21^FH\\^FDDATA VALIDADE :^FS" +
            "^FT293,349^A0N,23,21^FH\\^FDDATA FABRICA\\80\\C7O :^FS" +
            "^FT579,457^A0N,23,21^FH\\^FDPESO BRUTO/Un (Kg.):^FS" +
            "^FT291,456^A0N,23,21^FH\\^FDPESO L\\D6QUIDO/Cx. (Kg.):^FS" +
            "^FT56,457^A0N,23,21^FH\\^FDORDEM :^FS" +
            "^FT57,349^A0N,23,21^FH\\^FDLOTE :^FS" +
            "^PQ1,0,1,Y^XZ" +
            "^XA^ID000.GRF^FS^XZ";
            return text;
        }
    }
}
