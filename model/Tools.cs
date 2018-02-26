///////////////////////////////////////////////////////////////////////////
//  Copyright 2015 - 2020 Sabrina Prestigiacomo nabu@nabu.pt
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//  
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
///////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Drawing;
using System.Net;


namespace nabu
{
    public class point3D
    {
        public float x;
        public float y;
        public float z;

        public point3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class Polar
    {
        public Polar()
        {
        }

        public Polar(float M, float A)
        {
            this.M = M;
            this.A = A;
        }
        public float M { get; set; }
        public float A { get; set; }
    }

    public static class Tools
    {
        private static char coma = ' ';
        public static string startupPath;
        private static int fileIndex = 0;
        private static StreamWriter logFile;
        public static DateTime minValue = new DateTime(2000, 1, 1);
        public static System.Web.HttpServerUtility server = null;

        public static string getURLName(string s)
        {
            while (s.IndexOf('/') >= 0)
                s = s.Substring(s.IndexOf('/') + 1);
            return s;
        }

        public static string MapPath(string val)
        {
            return server.MapPath(val);
        }

        public static string HtmlEncode(string val)
        {
            return server.HtmlEncode(val);
        }

        public static string HTMLDecode(string val)
        {
            return server.HtmlDecode(val);
        }

        public static List<string> dictionary = new List<string>() {
            "El usuario no existe|es|El usuario no existe", 
            "La clave actual no corresponde|es|La clave actual no corresponde", 
            "La clave nueva no puede ser vacia|es|La clave nueva no puede ser vacia", 
            "Grupo hijo borrado|es|Grupo hijo borrado", 
            "Nuevo grupo hijo agregado|es|Nuevo grupo hijo agregado", 
            "Usuarios creados desahibilitados|es|Usuarios creados en [%1] desahibilitados, contacte al coordinador para habilitarlos", 
            "Usuario [%1] actualizado|es|Usuario [%1] actualizado", 
            "Usuario [%1] borrado|es|Usuario [%1] borrado", 
            "Usuario no habilitado|es|Usuario no habilitado para el grupo [%1]", 
            "Usuario o clave incorrectos|es|Usuario o clave incorrectos para el grupo [%1]", 
            "Usuario invalido|es|Usuario inválido, operacion registrada!", 
            "Documento simulado|es|Documento simulado", 
            "Simulacion|es|Simulacion!!!", 
            "Seleccione un nodo|es|Seleccione un nodo", 
            "Este debate ya ha alcanzado el consenso|es|Este debate ya ha alcanzado el consenso", 
            "Minimos usuarios implicados|es|M&iacute;nimos usuarios implicados debe estar entre 50 y 100", 
            "Maximos usuarios negados|es|M&aacute;ximos usuarios negados debe estar entre 0 y 50", 
            "Cantidad de flores|es|Cantidad de flores debe estar entre 1 y 20", 
            "Titulo|es|T&iacute;tulo", 
            "Etiqueta|es|Etiqueta", 
            "(Etiqueta en el arbol)|es|(Etiqueta en el &aacute;rbol)", 
            "Nivel en el arbol|es|Nivel en el &aacute;rbol", 
            "Consenso alcanzado|es|Consenso alcanzado", 
            "Cerrar|es|Cerrar", 
            "Prevista de propuesta|es|Prevista de propuesta", 
            "Cancelar|es|Cancelar", 
            "Revisar propuesta|es|Revisar propuesta", 
            "Permite corregir errores|es|Permite corregir errores", 
            "Crear propuesta|es|Crear propuesta", 
            "Crea la propuesta|es|Crea la propuesta", 
            "Enseña vista previa antes de proponer|es|Enseña vista previa antes de proponer", 
            "(Aparece en el pie del arbol)|es|(Aparece en el pi&eacute; del &aacute;rbol)", 
            "Enviar|es|Enviar", 
            "Manifiesto|es|Manifiesto", 
            "Este manifiesto reemplaza al anterior|es|Este manifiesto reemplaza al anterior", 
            "max|es|M&aacute;x", 
            "Fecha|es|Fecha", 
            "manifiesto.vision|es|La visi&oacute;n tiene un car&aacute;cter inspirador y motivador para el grupo. ¿Que visi&oacute;n tenemos del mundo?, ¿Como deber&iacute;an ser las cosas?", 
            "Proponer variante|es|Proponer variante", 
            "manifiesto.mision|es|La misi&oacute;n define cual es nuestra labor,  ¿qué hacemos?, ¿a qué nos dedicamos?, ¿en que sector social sucede?, ¿quiénes son nuestro p&uacute;blico objetivo?, ¿cuál es nuestro ámbito geogr&aacute;fico de acci&oacute;n?", 
            "No tiene flores para crear una propuesta|es|No tiene flores para crear una propuesta", 
            "Vision|es|Visi&oacute;n", 
            "Mision|es|Misi&oacute;n", 
            "Objetivos|es|Objetivos", 
            "manifiesto.objetivos|es|Los valores son principios éticos del grupo, son la base de nuestras pautas de comportamiento. Definen la personalidad del grupo. ¿cómo somos?, ¿en qué creemos?", 
            "manifiesto.servicios|es|¿Que servicios prestamos para alcanzar nuestra misi&oacute;n?. Estos servicios son el punto partida para la creaci&oacute;n de procesos operativos y grupos de trabajo. Lista los servicios con nombre y una breve descripci&oacute;n", 
            "Servicios|es|Servicios", 
            "Nivel [%1] no existe en este modelo|es|Nivel [%1] no existe en este modelo", 
            "Manifiesto actualizado en el grupo|es|Manifiesto actualizado en el grupo", 
            "Estructura organizativa actualizada|es|Estructura organizativa actualizada", 
            "El titulo del manifiesto no puede ser vacio|es|El t&iacute;tulo del manifiesto no puede ser vac&iacute;o", 
            "La vision no puede ser vacia|es|La visi&oacute;n no puede ser vac&iacute;a", 
            "La mision no ser vacia|es|La misi&oacute;n no ser vac&iacute;a", 
            "El objetivo no puede ser vacio|es|El objetivo no puede ser vac&iacute;o", 
            "proceso.introduccion|es|¿Cual es la sicuaci&oacute;n actual?, ¿Por qu&eacute; necesitamos una nueva estrategia?, ¿Que problemas resolver&aacute;? Ten en cuenta que la situati&oacute;n que describes represente un problema real del grupo", 
            "Introduccion|es|Introducci&oacute;n", 
            "proceso.consecuancias|es|¿Cuales son los consecuencias de eliminar este grupo de trabajo que habr&aacute; que enfrentar?", 
            "Objetivo a lograr|es|Objetivo a lograr", 
            "Descripcion|es|Descripci&oacute;n", 
            "proceso.descripcion|es|Describe brevemente los pasos a realizar usar esta estrategia. Menciona como se tratar&aacute;n los casos excepcionales. Una estrategia siempre debe generar un resultado, aunque sea un registro de fallo para su posterior an&aacute;lisis.", 
            "A quien va dirigido|es|A quien va dirigido", 
            "Conclusiones|es|Conclusiones", 
            "proceso.conclusiones|es|¿Que hemos aprendido con esta experiencia?", 
            "Entradas de proceso|es|Entradas de proceso", 
            "proceso.entradas|es|¿Como se provoca la ejecuci&oacute;n de este proceso? ¿Que necesita este proceso para poder ejecutarse? Un nuevo socio, un documento, una peticion personal, etc.", 
            "Definicion de la estrategia|es|Definici&oacute;n de la estrategia", 
            "proceso.definicion|es|Define paso por paso las tareas a realizar cuando se ejecute este proceso. Define como se tratar&aacute;n las excepciones. Una estrategia es un flujo de condiciones y tareas que contempla todas las alternativas posibles de manera que su ejecusi&oacute;n no admita discusiones sobre sus resultados. Un proceso operativo bien definido es parte importante de la definici&oacute;n operativa del grupo. Todas las actividades habituales del grupo deben definirse como procesos operativos.", 
            "Grupo de trabajo|es|Grupo de trabajo", 
            "proceso.grupo|es|¿Que grupo de trabajo usa esta estrategia?", 
            "Implantacion|es|Implantaci&oacute;n", 
            "proceso.implantacion|es|¿Que se debe hacer para implantar este proceso? Recursos, sitio fis&iacute;co, conocimientos y capacidades de las personas, formularios/hojas de datos/folletos informativos, software, etc.", 
            "Valoracion del resultado|es|Valoraci&oacute;n del resultado", 
            "proceso.valoracion|es|¿Como se medir&aacute; el resultado de esta estrategia? ¿cantidad de procesos ejecutados? ¿resultados obtenidos? ¿facilita la necesidad? ¿es facil de ejecutar?. <br>La valoraci&oacute;n de un proceso operativo debe ser cuantificable y facil de comprender. Debe quedar claro para todos si cumple con su objetivo o no.", 
            "Revision de valoracion del resultado|es|Revisi&oacute;n de valoraci&oacute;n del resultado", 
            "proceso.revision|es|¿Cuando se revisar&aacute; la definici&oacute;n de esta estrategia?", 
            "estrategia.eliminada|es|Estrategia eliminado del grupo de trabajo [%1] en la estructura organizativa", 
            "estrategia.actualizada|es|Estrategia actualizado en el grupo de trabajo [%1] en la estructura organizativa", 
            "estrategia.creada|es|Estrategia creado en el grupo de trabajo [%1] en la estructura organizativa", 
            "proceso.aquien|es|Quienes se beneficiaran con los resultados de utilizar esta estrategia", 
            "evento.introduccion|es|¿Cual es la situación que requiere de esta propuesta de evento? ¿porqu&eacute; se debe hacer? ¿cual es la motivaci&oacute;n? Ten en cuenta que la situati&oacute;n que describes represente al grupo", 
            "evento.objetivo|es|Describe que pretendes que logremos realizando este evento. Motiva al grupo con este objetivo. ¿es un deseo del grupo lograrlo?", 
            "evento.descripcion|es|Describe como ser&aacute; el evento. ¿show? ¿presentaciones? ¿ponentes? ¿musica? ¿actividades? ¿payasos? ¿catering?", 
            "event.aquien|es|A quienes va dirigido el evento y quienes se beneficiaran con los resultados", 
            "evento.lugar|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado", 
            "Materiales|es|Materiales", 
            "evento.materiales|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado", 
            "evento.transportes|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado. Software, desplazamientos, servicios, etc.", 
            "Organizacion|es|Organizaci&oacute;n", 
            "evento.organizacion|es|Describe como se debe organizar este evento, pasos a seguir, tiempos, desplazamientos, contrataciones, etc.", 
            "evento.fecha|es|Haz una propuesta de fecha para el evento",
            "evento.valoracion|es|¿Como se medir&aacute; el resultado de este evento? ¿entradas vendidas? ¿cantidad de personas presentes? ¿reconocimiento alcanzado? ¿efectos sociales? ¿publicdad?. <br>La valoraci&oacute;n del evento debe ser cuantificable y facil de comprender. Debe quedar claro si ha ido bien o no.",
            "accion|es|Acci&oacute;n",
            "Nombre|es|Nombre",
            "proceso.objetivo|es|¿Cuales son los objetivos del nuevo grupo de trabajo?",
            "accion.introduccion|es|¿Cual es la situación que requiere de esta propuesta de acci&oacute;n? ¿porqu&aacute; se debe actuar? ¿que mejorar&iacute;a? Ten en cuenta que la situati&oacute;n que describes represente al grupo",
            "accion.objetivo|es|Describe que pretendes que logremos como resultado de realizar la acci&oacute;n propuesta. Motiva al grupo con este objetivo. ¿es un deseo del grupo lograrlo?",
            "accion.descripcion|es|Describe brevemente como sera la acci&oacute;n a realizar. Esfuerzos, plazos, costos, desplazamientos, implicaciones, fases.",
            "accion.aquien|es|Quienes se beneficiar&aacute;n con los resultados",
            "accion.materiales|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado",
            "RRHH|es|RRHH",
            "accion.rrhh|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado",
            "Software|es|Software",
            "accion.software|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado. Software, desplazamientos, servicios, etc.",
            "Fases|es|Fases",
            "accion.fases|es|Describe las fases que se deben alcanzar para lograr el objetivo",
            "Presupuesto y plazo de entrega|es|Presupuesto y plazo de entrega",
            "accion.presupuesto|es|Estimaci&oacute;n de costos y plazos, el grupo debe conocer y aprobar estos valores",
            "grupoTrabajo.introduccion|es|¿Cual es la sicuaci&oacute;n actual?, ¿Por qu&eacute; necesitamos un nuevo grupo de trabajo?, ¿Que problemas resolver&aacute;? Ten en cuenta que la situati&oacute;n que describes represente al grupo",
            "grupoTrabajo.consecuencias|es|¿Cuales son los consecuencias de eliminar este grupo de trabajo que habr&aacute; que enfrentar?",
            "grupoTrabajo.objetivo|es|¿Cuales son los objetivos del nuevo grupo de trabajo?",
            "grupoTrabajo.descripcion|es|Describe brevemente las actividades a realizar para alcanzar los objetivos.",
            "grupoTrabajo.aquien|es|Quienes se beneficiaran con los resultados",
            "grupoTrabajo.conclusiones|es|¿Que hemos aprendido con esta experiencia?",
            "grupoTrabajo.recursos|es|¿Que recursos ser&aacute;n necesarios para el nuevo grupo de trabajo? Mobiliario, ordenadores, sitio de trabajo, etc.",
            "grupoTrabajo.capacidades|es|¿Que capacidades deben tener las personas que componen el nuevo grupo? Conocimientos, experiencias, etc.",
            "grupoTrabajo.integrantes|es|¿Quienes ser&aacute;n los integrantes del nuevo grupo de trabajo? Estos nombres sirven de propuesta o referencia. No implican realmente a las personas. Si no quieres implicar nombres reales di 'a definir'.",
            "grupoTrabajo.valoracion|es|¿Como se medir&aacute; el resultado de este grupo de trabajo? ¿cantidad de procesos ejecutados? ¿m&eacute;tricas de eficiancia? ¿reconocimiento de otras personas? ¿resultados concretos?. <br>La valoraci&oacute;n del desempeño del grupo debe ser cuantificable y facil de comprender. Debe quedar claro para todos si cumple con su objetivo o no.",
            "grupoTrabajo.revision|es|¿Con que periodicidad deben evaluarse los resultados alcanzados.",
            "grupoTrabajo.eliminado|es|Grupo de trabajo eliminado de la estructura organizativa",
            "grupoTrabajo.actualizado|es|Grupo de trabajo actualizado en la estructura organizativa",
            "grupoTrabajo.creado|es|Grupo de trabajo creado en la estructura organizativa",
            "Crear nueva estrategia|es|Crear nuevo estrategia",
            "Modificar estrategia|es|Modificar estrategia",
            "Eliminar estrategia|es|Eliminar estrategia",
            "Crear nuevo grupo de trabajo|es|Crear nuevo grupo de trabajo",
            "Modificar grupo de trabajo|es|Modificar grupo de trabajo",
            "Eliminar grupo de trabajo|es|Eliminar grupo de trabajo",
            "Comentarios|es|Comentarios",           
            "Nombre de nodo no puede ser vacio|es|Nombre de nodo no puede ser vacio",           
            "El usuario no existe o no esta habilitado|es|El usuario no existe o no esta habilitado",           
            "El nodo no existe|es|El nodo no existe",           
            "accion.evaluacion.p1|es|&iquest;El problema a enfrentar estaba bien definido?",
            "accion.evaluacion.p2|es|&iquest;El objetivo era adecuado para enfrentar el problema?",
            "accion.evaluacion.p3|es|&iquest;Se han destinado los recursos previstos?",
            "accion.evaluacion.p4|es|&iquest;La fases en que se realiz&oacute; eran adecuadas?",
            "accion.evaluacion.p5|es|&iquest;El costo y plazo ha sido seg&uacute;n lo planeado?",
            "accion.evaluacion.p6|es|&iquest;Crees que se requiere otro ciclo de Accion para resolver este tema?",
            "enlace.evaluacion.p1|es|&iquest;Este articulo esta relacionado con los intereses del grupo?",
            "enlace.evaluacion.p2|es|&iquest;Tiene referencias s&oacute;lidas?",
            "enlace.evaluacion.p3|es|&iquest;Se esta tratando este tema en el grupo?",
            "enlace.evaluacion.p4|es|&iquest;Deberia tratarse este tema en el grupo?",
            "enlace.evaluacion.p5|es|&iquest;Crees que este tema deberia generar un ciclo de Accion?",
            "accion.evaluacion.tip1|es|",
            "accion.evaluacion.tip2|es|",
            "accion.evaluacion.tip3|es|",
            "accion.evaluacion.tip4|es|",
            "accion.evaluacion.tip5|es|",
            "accion.evaluacion.tip6|es|",
            "enlace.evaluacion.tip1|es|",
            "enlace.evaluacion.tip2|es|",
            "enlace.evaluacion.tip3|es|",
            "enlace.evaluacion.tip4|es|",
            "enlace.evaluacion.tip5|es|",
            "alhijo.documento.introduccion.tip|es|",
            "alhijo.documento.situacionactual.tip|es|",
            "alhijo.documento.propuesta.tip|es|",
            "alhijo.documento.situaciondeseada.tip|es|",
            "alpadre.documento.introduccion.tip|es|",
            "alpadre.documento.situacionactual.tip|es|",
            "alpadre.documento.propuesta.tip|es|",
            "alpadre.documento.situaciondeseada.tip|es|",
            "alhijo.evaluacion.p1|es|&iquest;El problema a enfrentar esta bien definido?",
            "alhijo.evaluacion.p2|es|&iquest;La situaci&oacute;n actual se corresponde con la realidad?",
            "alhijo.evaluacion.p3|es|&iquest;La propuesta deberia analizarce?",
            "alhijo.evaluacion.p4|es|&iquest;La situaci&oacute;n deseada es asumible?",
            "alhijo.evaluacion.p5|es|&iquest;Este tema deberia atenderse en este grupo?",
            "alhijo.evaluacion.tip1|es|",
            "alhijo.evaluacion.tip2|es|",
            "alhijo.evaluacion.tip3|es|",
            "alhijo.evaluacion.tip4|es|",
            "alhijo.evaluacion.tip5|es|",
            "alpadre.evaluacion.p1|es|&iquest;El problema a enfrentar esta bien definido?",
            "alpadre.evaluacion.p2|es|&iquest;La situaci&oacute;n actual se corresponde con la realidad?",
            "alpadre.evaluacion.p3|es|&iquest;La propuesta deberia analizarce?",
            "alpadre.evaluacion.p4|es|&iquest;La situaci&oacute;n deseada es asumible?",
            "alpadre.evaluacion.p5|es|&iquest;Este tema deberia atenderse en este grupo?",
            "alpadre.evaluacion.tip1|es|",
            "alpadre.evaluacion.tip2|es|",
            "alpadre.evaluacion.tip3|es|",
            "alpadre.evaluacion.tip4|es|",
            "alpadre.evaluacion.tip5|es|",
            "Palabras clave|es|Palabras clave",
            "Evaluación de comunicado intergrupal|es|Evaluación de comunicado intergrupal",
            "Documento de comunicado a evaluar|es|Documento de comunicado a evaluar",
            "Prevista de evaluacion|es|Prevista de evaluaci&oacute;n",
            "Revisar evaluacion|es|Revisar evaluaci&oacute;n",
            "Crear evaluacion|es|Crear evaluaci&oacute;n",
            "Evaluación de Accion|es|Evaluaci&oacute;n de Accion",
            "Documento de resultado a evaluar|es|Documento de resultado a evaluar",
            "Evaluacion de Enlace|es|Evaluaci&oacute;n de Enlace",
            "Nombre de la publicacion|es|Nombre de la publicaci&oacute;n",
            "URL de la publicacion|es|URL de la publicaci&oacute;n",
            "Autor de la publicacion|es|Autor de la publicaci&oacute;n",
            "accion.responsable|es|Responsable de proyecto",
            "accion.revision|es|Revisi&oacute;n",
            "Responsable|es|Responsable",
            "accion.creada|es|Accion creada",
            "Nueva decision [%1]|es|Nueva decisi&oacute;n [%1]",
            "Recursos|es|Recursos",
            "Capacidaddes|es|Capacidaddes",
            "Integrantes|es|Integrantes",
            "Consecuencias|es|Consecuencias",
            "Comunicado al grupo padre|es|Comunicado al grupo padre",            
            "Representante|es|Representante",
            "Prevista de evaluacion|es|Prevista de evaluaci&oacute;n",
            "Revisar evaluacion|es|Revisar evaluaci&oacute;n",
            "Crear evaluacion|es|Crear evaluaci&oacute;n",
            "Evaluación de comunicado intergrupal|es|Evaluaci&oacute;n de comunicado intergrupal",     
            "Nueva Acta publicada|es|Nueva Acta publicada",

            "El usuario no existe|ct|La usu&agrave;ria no existeix", 
            "La clave actual no corresponde|ct|La clau actual no correspon", 
            "La clave nueva no puede ser vacia|ct|La clau nova no pot estar buida", 
            "Grupo hijo borrado|ct|Grup filla borrat", 
            "Nuevo grupo hijo agregado|ct|Nou grup filla agregat", 
            "Usuarios creados desahibilitados|ct|Usu&agrave;ries creades a [%1] deshabilitades, contacta la jardinera per a habilitar-les", 
            "Usuario [%1] actualizado|ct|Usu&agrave;ria [%1] actualitzada", 
            "Usuario [%1] borrado|ct|Usu&agrave;ria [%1] borrada", 
            "Usuario no habilitado|ct|Usu&agrave;ria no habilitada per al grup [%1]", 
            "Usuario o clave incorrectos|ct|Usu&agrave;ria o clau incorrectes per al grup [%1]", 
            "Usuario invalido|ct|Usuària inv&agrave;lida, operaci&oacute; registrada!", 
            "Documento simulado|ct|Document simulat", 
            "Simulacion|ct|Simulació!!!", 
            "Seleccione un nodo|ct|Selecciona un node", 
            "Este debate ya ha alcanzado el consenso|ct|Aquest debat ja ha arribat al consens", 
            "Minimos usuarios implicados|ct|El m&iacute;nim d'usu&agrave;ries implicades ha d'estar entre 50 i 100", 
            "Maximos usuarios negados|ct|El m&agrave;xim d'usu&agrave;ries negades ha d'estar entre 0 i 50", 
            "Cantidad de flores|ct|La Quantitat de flors ha d'estar entre 1 i 20", 
            "Titulo|ct|T&iacute;tol", 
            "Etiqueta|ct|Etiqueta", 
            "(Etiqueta en el arbol)|ct|(Etiqueta a l'arbre)", 
            "Nivel en el arbol|ct|Nivell a l'arbre", 
            "Consenso alcanzado|ct|Consens aconseguit", 
            "Cerrar|ct|Tancar", 
            "Prevista de propuesta|ct|Prevista de proposta", 
            "Cancelar|ct|Cancel&middot;lar", 
            "Revisar propuesta|ct|Revisar proposta", 
            "Permite corregir errores|ct|Permet corregir errors", 
            "Crear propuesta|ct|Crear proposta", 
            "Crea la propuesta|ct|Crea la proposta", 
            "Enseña vista previa antes de proponer|ct|Ensenya vista pr&egrave;via abans de proposar", 
            "(Aparece en el pie del arbol)|ct|(Apareix al peu de l'arbre)", 
            "Enviar|ct|Enviar", 
            "Manifiesto|ct|Manifest", 
            "Este manifiesto reemplaza al anterior|ct|Aquest manifest reemplaça l'anterior", 
            "max|ct|Màx", 
            "Fecha|ct|Data", 
            "manifiesto.vision|ct|La visi&oacute; t&eacute; un car&agrave;cter inspirador i motivador per al grup. Quina visi&oacute; tenim del m&oacute;n?, Com haurien de ser, les coses?",
            "Proponer variante|ct|Proposar variant", 
            "manifiesto.mision|ct|La missi&oacute; defineix quina &eacute;s la nostra feina, qu&egrave; fem?, a qu&egrave; ens dediquem?, en quin sector social succeeix?, qui &eacute;s el nostre p&uacute;blic objectiu?, quin &eacute;s el nostre &agrave;mbit geogr&agrave;fic d'actuaci&oacute;?",
            "No tiene flores para crear una propuesta|ct|No tens flors per a crear una proposta", 
            "Vision|ct|Visi&oacute;", 
            "Mision|ct|Missi&oacute;", 
            "Valores|ct|Valors", 
            "manifiesto.valores|ct|Els valors s&oacute;n principis &egrave;tics del grup, s&oacute;n la base de les nostres pautes de comportament. Defineixen la personalitat del grup. Com som?, en qu&egrave; creiem?",
            "manifiesto.servicios|ct|Quins serveis prestem per a aconseguir la nostra missi&oacute;? Aquests serveis s&oacute;n el punt de partida per a la creaci&oacute; de processos operatius i grups de treball. Llista els serveis amb nom i una breu descripci&oacute;.",
            "Servicios|ct|Serveis", 
            "Nivel [%1] no existe en este modelo|ct|El nivell [%1] no existeix en aquest model", 
            "Manifiesto actualizado en el grupo|ct|Manifest actualizat al grup", 
            "Estructura organizativa actualizada|ct|Estructura organitzativa actualitzada", 
            "El titulo del manifiesto no puede ser vacio|ct|El t&iacute;tol del manifest no pot estar buit", 
            "La vision no puede ser vacia|ct|La visi&oacute; no pot estar buida", 
            "La mision no ser vacia|ct|La missi&oacutre; no pot estar buida", 
            "El objetivo no puede ser vacio|ct|L'objectiu no pot estar buit", 
            "proceso.introduccion|ct|Quina &eacute;s la situaci&oacute; actual?, Per qu&egrave; necessitem un nou procés operatiu?, Quins problemes resoldr&agrave;? Assegura't que la situaci&oacute; que descrius representi un problema real del grup",
            "Introduccion|ct|Introducci&oacute;", 
            "proceso.consecuancias|ct|Quines s&oacute;n les conseq&uuml;&egrave;ncies d'eliminar aquest grup de treball que haur&iacute;em d'afrontar?", 
            "Objetivo a lograr|ct|Objectiu a aconseguir", 
            "Descripcion|ct|Descripci&oacute;", 
            "proceso.descripcion|ct|Descriu breument els passos a realitzar a l'executar el proc&eacute;s operatiu. Esmenta com es tractaran els casos excepcionals. Un proc&eacute;s operatiu sempre ha de generar un resultat, encara que sigui un registre de fallada per al seu posterior an&agrave;lisi.",
            "A quien va dirigido|ct|A qui va dirigit", 
            "Conclusiones|ct|Conclusions", 
            "proceso.conclusiones|ct|Què hem apr&egrave;s amb aquesta experi&egrave;ncia?",
            "Entradas de proceso|ct|Entrades de proc&eacute;s", 
            "proceso.entradas|ct|Com es provoca l'execuci&oacute; d'aquest proc&eacute;s? Qu&egrave; necessita aquest proc&eacute;s per a poder executar-se? Una nova s&ograve;cia, un document, una petici&oacute; personal, etc.",
            "Definicion de la estrategia|ct|Definici&oacute; de la estrategia", 
            "proceso.definicion|ct|Defineix pas per pas les tasques a realitzar quan s'executi aquest proc&eacute;s. Defineix com es tractaran les excepcions. Un proc&eacute;s operatiu &eacute;s un fluxe de condicions i tasques que contempla totes les alternatives possibles de manera que la seva execució no admeti discussions sobre els seus resultats. Un proc&eacute;s operatiu ben definit &eacute;s part important de la definici&oacute; operativa del grup. Totes les activitats habituals del grup han de definir-se com a processos operatius.",
            "Grupo de trabajo|ct|Grup de treball", 
            "proceso.grupo|ct|Quin grup de treball fa servir aquest proc&eacute;s operatiu?", 
            "Implantacion|ct|Implantaci&oacute;", 
            "proceso.implantacion|ct|Qu&egrave; s'ha de fer per a implantar aquest proc&eacute;s? Recursos, lloc f&iacute;sic, coneixements i capacitats de les persones, formularis/fulls de dades/fullets informatius, programari, etc.",
            "Valoracion del resultado|ct|Valoraci&oacute; del resultat", 
            "proceso.valoracion|ct|Com es medir&agrave; el resultat d'aquest proc&eacute;s operatiu? La quantitat de processos executats? Els resultats obtinguts? Facilita la necessitat? És fàcil d'executar?<br>La valoraci&oacute; d'un proc&eacute;s operatiu ha de ser quantificable i f&agrave;cil de comprendre. Ha de quedar clar per a totes si compleix amb el seu objectiu o no.",
            "Revision de valoracion del resultado|ct|Revisi&oacute; de la valoraci&oacute; del resultat",
            "proceso.revision|ct|Quan es revisar&agrave; la definici&oacute; d'aquest proc&eacute;s operatiu?",
            "estrategia.eliminada|ct|Estrategia eliminat del grup de treball [%1] a l'estructura organitzativa",
            "estrategia.actualizada|ct|Estrategia actualitzat al grup de treball [%1] a l'estructura organitzativa",
            "estrategia.creada|ct|Estrategia creat al grup de treball [%1] a l'estructura organitzativa",
            "proceso.aquien|ct|Qui es beneficiar&agrave; amb els resultats de fer servir aquest proc&eacute;s operatiu",
            "evento.introduccion|ct|Quina &eacute;s la situaci&oacute; que requereix d'aquesta proposta d'event? Per qu&egrave; s'ha de fer? Quina &eacute;s la motivaci&oacute;? Assegura't que la situaci&oacute; que descrius representi el grup",
            "evento.objetivo|ct|Descriu qu&egrave; vols que aconseguim realitzant aquest event. Motiva el grup amb aquest objectiu. &Eacute;s un desig del grup, aconseguir-ho?",
            "evento.descripcion|ct|Descriu com ser&agrave; l'event. Un espectacle? Presentacions? Ponents? M&uacute;sica? Activitats? Pallasses? &Agrave;pats?",
            "event.aquien|ct|A qui va dirigit l'event i qui es beneficiar&aacute; amb els resultats",
            "evento.lugar|ct|Descriu el lloc on es far&aacute; l'event sense oblidar un pressupost estimat",
            "Materiales|ct|Materials", 
            "evento.materiales|ct|Descriu els recursos que seran necessaris sense oblidar un pressupost estimat",
            "evento.transportes|ct|Descriu els transports que seran necessaris sense oblidar un pressupost estimat. Programari, desplaçaments, serveis, etc.",
            "Organizacion|ct|Organitzaci&oacute;", 
            "evento.organizacion|ct|Descriu com s'ha d'organitzar aquest event, passos a seguir, temps, desplaçaments, contractacions, etc.",
            "evento.fecha|ct|Fes una proposta de data per a l'event",
            "evento.valoracion|ct|Com es medir&agrave; el resultat d'aquest event? Entrades venudes? Quantitat de persones presents? Reconeixement aconseguit? Efectes socials? Publicitat?<br>La valoraci&oacute; de l'event ha de ser quantificable i f&agrave;cil de comprendre. Ha de quedar clar si ha anat b&eacute; o no.",
            "accion|ct|Acci&oacute;",
            "Nombre|ct|Nom",
            "proceso.objetivo|ct|Quins s&oacute;n els objectius del nou grup de treball?",
            "accion.introduccion|ct|Quina &eacute;s la situaci&oacute; que requereix d'aquesta proposta d'acci&oacute;? Per qu&egrave; s'ha d'actuar? Qu&egrave; millorar&agrave;? Assegura't que la situaci&oacute; que descrius representi el grup",
            "accion.objetivo|ct|Descriu qu&egrave; vols que aconseguim com a resultat de realitzar l'acci&oacute; proposada. Motiva el grup amb aquest objectiu. &Eacute;s un desig del grup aconseguir-ho?",
            "accion.descripcion|ct|Descriu breument com ser&agrave; l'acci&oacute; a realitzar. Esforços, terminis, costos, desplaçaments, implicacions, fases.",
            "accion.aquien|ct|Qui es beneficiar&agrave; amb els resultats",
            "accion.materiales|ct|Descriu els recursos que seran necessaris sense oblidar un pressupost estimat",
            "RRHH|ct|RRHH",
            "accion.rrhh|ct|Descriu els recursos humans que seran necessaris sense oblidar un pressupost estimat",
            "Software|ct|Programari",
            "accion.software|ct|Descriu els recursos de programari que seran necessaris sense oblidar un pressupost estimat. Programari, serveis, etc.",
            "Fases|ct|Fases",
            "accion.fases|ct|Descriu les fases que s'han de seguir per a aconseguir l'objectiu",
            "Presupuesto y plazo de entrega|ct|Pressupost i termini d'entrega",
            "accion.presupuesto|ct|Estimaci&oacute; de costos i terminis. El grup ha de con&egrave;ixer i aprovar aquests valors.",
            "grupoTrabajo.introduccion|ct|Quina &eacute;s la situaci&eacute; actual? Per qu&egrave; necessitem un nou grup de treball? Quins problemes resoldr&agrave;? Assegura't que la situaci&oacute; que descrius representi el grup",
            "grupoTrabajo.consecuencias|ct|Quines s&oacute;n les conseq&uuml;&egrave;ncies d'eliminar aquest grup de treball que haurem d'afrontar?",
            "grupoTrabajo.objetivo|ct|Quins s&oacute;n els objectius del nou grup de treball?",
            "grupoTrabajo.descripcion|ct|Descriu breument les activitats per realitzar per a aconseguir els objectius.",
            "grupoTrabajo.aquien|ct|Qui es beneficiar&agrave; amb els resultats",
            "grupoTrabajo.conclusiones|ct|Qu&egrave; hem apr&egrave;s amb aquesta experi&egrave;ncia?",
            "grupoTrabajo.recursos|ct|Quins recursos seran necessaris per al nou grup de treball? Mobiliari, ordinadors, lloc de treball, etc.",
            "grupoTrabajo.capacidades|ct|Quines capacitats han de tenir les persones que componen el nou grup? Coneixements, experiències, habilitats, etc.",
            "grupoTrabajo.integrantes|ct|Qui ser&egrave; les integrants del nou grup de treball? Aquests noms serveixen de proposta o refer&egrave;ncia. No impliquen realment les persones. Si no vols implicar noms reals digues 'per definir'.",
            "grupoTrabajo.valoracion|ct|Com es medir&egrave; el resultat d'aquest grup de treball? Quantitat de processos executats? M&egrave;triques d'efici&egrave;ncia? Reconeixement d'altres persones? Resultats concrets?<br>La valoraci&oacute; de l'acompliment del grup ha de ser quantificable i f&agrave;cil de comprendre. Ha de quedar clar per a totes si compleix amb el seu objectiu o no.",
            "grupoTrabajo.revision|ct|Amb quina periodicitat han d'avaluar-se els resultats obtinguts?",
            "grupoTrabajo.eliminado|ct|Grup de treball eliminat de l'estructura organitzativa",
            "grupoTrabajo.actualizado|ct|Grup de treball actualitzat a l'estructura organitzativa",
            "grupoTrabajo.creado|ct|Grup de treball creat a l'estructura organitzativa",
            "Servicios|ct|Serveis",
            "Crear nueva estrategia|ct|Crear nou estrategia",
            "Modificar estrategia|ct|Modificar estrategia",
            "Eliminar estrategia|ct|Eliminar estrategia",
            "Crear nuevo grupo de trabajo|ct|Crear nou grup de treball",
            "Modificar grupo de trabajo|ct|Modificar grup de treball",
            "Eliminar grupo de trabajo|ct|Eliminar grup de treball",           
            "Comentarios|ct|Comentaris",           
            "Nombre de nodo no puede ser vacio|ct|Nom de node no pot ser buit",           
            "El usuario no existe o no esta habilitado|ct|L'usuari no existeix o no aquesta habilitat",           
            "El nodo no existe|ct|Aquest node encara no existeix",           

            "El usuario no existe|en|Username does not exist", 
            "La clave actual no corresponde|en|Key does not match", 
            "La clave nueva no puede ser vacia|en|New key can not be empty", 
            "Grupo hijo borrado|en|Child group deleted ", 
            "Nuevo grupo hijo agregado|en|New child group added", 
            "Usuarios creados desahibilitados|en|Disabled users created in [%1], contact the gardener to enable them", 
            "Usuario [%1] actualizado|en|User [%1] updated", 
            "Usuario [%1] borrado|en|User [%1] deleted", 
            "Usuario no habilitado|en|User disabled for group [%1]", 
            "Usuario o clave incorrectos|en|User or password incorrect for the group [%1]", 
            "Usuario invalido|en|Ivalid user, operation registered!", 
            "Documento simulado|en|Simulated document", 
            "Simulacion|en|Simulation!!!", 
            "Seleccione un nodo|en|Select a tree node", 
            "Este debate ya ha alcanzado el consenso|en|This discussion has already reached consensus", 
            "Minimos usuarios implicados|en|Minimal users involved should be between 50 and 100", 
            "Maximos usuarios negados|en|Maximum denied users must be between 0 and 50", 
            "Cantidad de flores|en|Number of flowers must be between 1 and 20", 
            "Titulo|en|Title", 
            "Etiqueta|en|Label", 
            "(Etiqueta en el arbol)|en|(Label in the tree)", 
            "Nivel en el arbol|en|Tree level", 
            "Consenso alcanzado|en|Consensus reached", 
            "Cerrar|en|Close", 
            "Prevista de propuesta|en|Proposal preview", 
            "Cancelar|en|Cancel", 
            "Revisar propuesta|en|Check proposal", 
            "Permite corregir errores|en|Allows to correct errors", 
            "Crear propuesta|en|Create proposal", 
            "Crea la propuesta|en|Create proposal", 
            "Enseña vista previa antes de proponer|en|Teach preview before proposing", 
            "(Aparece en el pie del arbol)|en|(Appears at the foot of the tree)", 
            "Enviar|en|Send", 
            "Manifiesto|en|Manifest", 
            "Este manifiesto reemplaza al anterior|en|This manifest replaces the previous one", 
            "max|en|Max", 
            "Fecha|en|Date", 
            "manifiesto.vision|en|The vision has an inspiring and motivating character for the group. What vision do we have of the world? How should things be?", 
            "Proponer variante|en|Suggest a variant", 
            "manifiesto.mision|en|The mission defines what our work is, what we do, what social sector happens, who are our target audience, what is our geographic scope of action.", 
            "No tiene flores para crear una propuesta|en|No flowers to create a proposal", 
            "Vision|en|View", 
            "Mision|en|Mission", 
            "Valores|en|Values", 
            "manifiesto.valores|en|Values ​​are ethical principles of the group, are the basis of our patterns of behavior. They define the personality of the group. How are we? What do we believe in?", 
            "manifiesto.servicios|en|What services do we provide to achieve our mission? These services are the starting point for the creation of operational processes and working groups. List named services and a brief description", 
            "Servicios|en|Services", 
            "Nivel [%1] no existe en este modelo|en|Level [%1] does not exist on this model", 
            "Manifiesto actualizado en el grupo|en|Manifesto updated in the group", 
            "Estructura organizativa actualizada|en|Organizational structure updated", 
            "El titulo del manifiesto no puede ser vacio|en|The title of the manifest can not be empty", 
            "La vision no puede ser vacia|en|Vision can not be empty", 
            "La mision no ser vacia|en|Mission can not be empty", 
            "El objetivo no puede ser vacio|en|Goal can not be empty", 
            "proceso.introduccion|en|What is the current situation? Do we need a new operational process? What problems will it solve? Note that the situation you describe represents a real problem for the group", 
            "Introduccion|en|Introduction", 
            "proceso.consecuancias|en|What are the consequences of eliminating this group of work, What to face?", 
            "Objetivo a lograr|en|Goal to reach", 
            "Descripcion|en|Description", 
            "proceso.descripcion|en|Briefly describes the steps to perform when executing the operating process. Mention how exceptional cases will be treated. An operational process must always generate a result, even if it is a fault record for later analysis.", 
            "A quien va dirigido|en|Who is it addressed to?", 
            "Conclusiones|en|Conclusions", 
            "proceso.conclusiones|en|What have we learned from this experience?", 
            "Entradas de proceso|en|Process inputs", 
            "proceso.entradas|en|How is the implementation of this process triggered? What does this process need to be able to run? A new partner, a document, a personal petition, etc.", 
            "Definicion de la estrategia|en|Defining the strategy", 
            "proceso.definicion|en|Defines step by step the tasks to perform when this process is executed. Define how exceptions will be treated. An operational process is a flow of conditions and tasks that contemplates all the possible alternatives so that its execution does not admit discussions about its results. A well defined operational process is an important part of the operational definition of the group. All the usual activities of the group should be defined as operational processes.", 
            "Grupo de trabajo|en|Workgroup", 
            "proceso.grupo|en|What working group uses this operational process?", 
            "Implantacion|en|Implantation", 
            "proceso.implantacion|en|What should be done to implement this process? Resources, site, knowledge and skills of people, forms / data sheets / information leaflets, software, etc.", 
            "Valoracion del resultado|en|Evaluation of the result", 
            "proceso.valoracion|en|How to measure the result of this operational process? Number of processes executed? Results obtained? Does it facilitate the need? Is it easy to execute?. <br> The assessment of an operational process must be quantifiable and easy to understand. It should be clear to everyone if it reach the goal or not.", 
            "Revision de valoracion del resultado|en|Valuation review of the result", 
            "proceso.revision|en|When will this process be reviewed?", 
            "estrategia.eliminada|en|Strategy removed from workgroup [%1] in organizational structure", 
            "estrategia.actualizada|en|Updated strategy in the workgroup [%1] in the organizational structure", 
            "estrategia.creada|en|Strategy created in the workgroup [%1] in the organizational structure", 
            "proceso.aquien|en|Who will benefit from the results of using this operational process", 
            "evento.introduccion|en|What is the situation that requires this event proposal? Why should it be done? What is the motivation? Please note that the situation you are describing represents the group", 
            "evento.objetivo|en|Describe what you want us to accomplish by holding this event. Motivate the group with this goal. Is it a group desire to achieve it?", 
            "evento.descripcion|en|Describe how it will be the event. Show? presentations? Speakers? music? activities? Clowns Catering?", 
            "event.aquien|en|To whom the event is addressed and who will benefit from the results", 
            "evento.lugar|en|Describe the resources that will be needed without forgetting an estimated budget", 
            "Materiales|en|Materials", 
            "evento.materiales|en|Describe the resources that will be needed without forgetting an estimated budget", 
            "evento.transportes|en|Describe the resources that will be needed without forgetting an estimated budget. Software, travel, services, etc.", 
            "Organizacion|en|Organization", 
            "evento.organizacion|en|Describe how to organize this event, steps to follow, times, travel, hiring, etc.", 
            "evento.fecha|en|Make a date proposal for the event",
            "evento.valoracion|en|How to measure the outcome of this event? Tickets sold? Number of people present? Recognition achieved? Social effects? <br> The assessment of the event must be quantifiable and easy to understand. It should be clear whether it went well or not.",
            "accion|en|Aciton",
            "Nombre|en|Name",
            "proceso.objetivo|en|What are the objectives of the new working group?",
            "accion.introduccion|en|What is the situation that requires this proposal of action? Why Should act? What to improve? Please note that the situation you are describing represents the group",
            "accion.objetivo|en|Describe what you want us to accomplish as a result of carrying out the proposed action. Motivate the group with this goal. Is it a group desire to achieve it?",
            "accion.descripcion|en|Describe briefly how the action will be performed. Efforts, deadlines, costs, displacements, implications, phases.",
            "accion.aquien|en|Who will benefit from the results",
            "accion.materiales|en|Describe the resources that will be needed without forgetting an estimated budget",
            "RRHH|en|RRHH",
            "accion.rrhh|en|Describe the resources that will be needed without forgetting an estimated budget",
            "Software|en|Software",
            "accion.software|en|Describe the resources that will be needed without forgetting an estimated budget. Software, travel, services, etc.",
            "Fases|en|Phases",
            "accion.fases|en|Describe the steps to achieve to achieve the goal",
            "Presupuesto y plazo de entrega|en|Budget and deadline",
            "accion.presupuesto|en|Estimation of costs and deadlines, the group must know and approve these values",
            "grupoTrabajo.introduccion|en|What is the current situation? Do we need a new working group? What problems do we have to solve? Please note that the situation you are describing represents the group",
            "grupoTrabajo.consecuencias|en|What are the consequences of eliminating this group of work, What to face?",
            "grupoTrabajo.objetivo|en|What are the objectives of the new working group?",
            "grupoTrabajo.descripcion|en|Briefly describe the activities to achieve the objectives.",
            "grupoTrabajo.aquien|en|Who will benefit from the results",
            "grupoTrabajo.conclusiones|en|What have we learned from this experience?",
            "grupoTrabajo.recursos|en|What resources will be needed for the new working group? Furniture, computers, workstation, etc.",
            "grupoTrabajo.capacidades|en|What skills should the people who make up the new group have? Knowledge, experience, etc.",
            "grupoTrabajo.integrantes|en|Who will be the members of the new working group?.",
            "grupoTrabajo.valoracion|en|How to measure the result of this working group? Number of processes executed? Efficacy methods? Recognition of other people? Concrete results? <br> The assessment of group performance should be quantifiable and easy to understand. It should be clear to everyone whether or not you meet its goal.",
            "grupoTrabajo.revision|en|How often should the results achieved be evaluated?",
            "grupoTrabajo.eliminado|en|Deleted workgroup of organizational structure",
            "grupoTrabajo.actualizado|en|Updated working group in organizational structure",
            "grupoTrabajo.creado|en|Working group created in the organizational structure",
            "Crear nueva estrategia|en|Create new strategy",
            "Modificar estrategia|en|Modify strategy",
            "Eliminar estrategia|en|Delete strategy",
            "Crear nuevo grupo de trabajo|en|Create new workgroup",
            "Modificar grupo de trabajo|en|Modify workgroup",
            "Eliminar grupo de trabajo|en|Delete workgroup",
            "Comentarios|en|Comments"           ,
            "Nombre de nodo no puede ser vacio|en|Name of node can not be empty",           
            "El usuario no existe o no esta habilitado|en|User des not exist or is disabled",           
            "El nodo no existe|en|Node does not exist"
        };


        public static string tr(string key, string p1, string idioma)
        {
            string ret = tr(key, idioma);
            return ret.Replace("%1", p1);
        }

        public static string tr(string key, string idioma)
        {
            foreach (string entry in dictionary)
            {
                if (entry.ToLower().StartsWith(key.ToLower() + "|" + idioma.ToLower() + "|"))
                {
                    var ret = entry.Substring(key.Length + idioma.Length + 2);
                    if (ret != "")
                        return ret;
                    else
                    {
                        registrarDictionaryException(key, idioma);
                        return "[" + key + "]";
                    }
                }
            }
            registrarDictionaryException(key, idioma);
            return "[" + key + "]";
        }

        private static void registrarDictionaryException(string key, string idioma)
        {
            System.IO.File.AppendAllText(startupPath + "\\logDictionary.txt", DateTime.Now.ToString("dd/MM/yy") + " " + key + "|" + idioma + "\r\n"); 
        }


        public static string getHttp(string sincroURL)
        {
            WebRequest request = WebRequest.Create(sincroURL);
            System.IO.StreamReader sr = new System.IO.StreamReader(request.GetResponse().GetResponseStream());
            string ret = sr.ReadToEnd();
            return ret;
        }

        public static string getHttp(string sincroURL, string post)
        {
            WebClient wc = new WebClient();
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            post = System.Web.HttpUtility.UrlEncode(post);
            string ret = wc.UploadString(sincroURL, "sinf=" + post);
            return ret;
        }

        public static string getFileHttp(string sincroURL, string fname)
        {
            WebClient wc = new WebClient();
            Byte[] res = wc.UploadFile(sincroURL, fname);
            return System.Text.Encoding.UTF8.GetString(res);
        }

        public static string dateToString(DateTime d)
        {
            //devuelvo formato texto ordenable
            return d.Year.ToString("0000") + "/" + d.Month.ToString("00") + "/" + d.Day.ToString("00") + " " + d.Hour.ToString("00") + ":" + d.Minute.ToString("00") + ":" + d.Second.ToString("00");
        }

        public static string writeTempImgFile(Image img)
        {
            Bitmap bmp;
            if (img is Bitmap) bmp = (Bitmap)img; else bmp = new Bitmap(img);
            string fn = getNewTempFilename(".gif");
            bmp.Save(startupPath + "\\wwwroot\\temp\\" + fn, System.Drawing.Imaging.ImageFormat.Gif);
            return fn;
        }

        public static string jsonFormat(Single s)
        {
            return s.ToString().Replace(",", ".");
        }

        public static System.Net.IPAddress solveIP(string ip)
        {
            if (ip.ToLower() == "any")
                return System.Net.IPAddress.Any;
            else
                return System.Net.Dns.Resolve(ip).AddressList[0];
        }

        public static void CloseLog()
        {
            if (logFile != null)
                logFile.Close();
        }

        public static void Log(string logMessage, string ip)
        {
            if (logFile == null)
                logFile = File.AppendText(startupPath + "\\log.txt");

            lock (logFile)
            {
                DateTime n = DateTime.Now;
                logFile.Write(n.ToString("dd/MM/yy") + ";" + n.ToShortTimeString() + ";" + ip + ";" + logMessage + "\r\n");
            }
        }

        public static void Log(string logMessage)
        {
            Log(logMessage, "");
        }

        public static string getNewTempFilename(string extension)
        {
            fileIndex++;
            while (System.IO.File.Exists(startupPath + "\\wwwroot\\temp\\temp" + fileIndex + extension))
            {
                fileIndex++;
            }
            return "temp" + fileIndex + extension;
        }

        public static float ParseF(string n)
        {
            if (coma == ' ') coma = (0.0).ToString("0.0")[1];

            return float.Parse(n.Replace('.', coma));
        }

        public static Polar toPolar(int x, int y)
        {
            Polar ret = new Polar();
            ret.M = (int)Math.Sqrt(x * x + y * y);
            if (x == 0)
                if (y > 0) ret.A = 90; else ret.A = 270;
            else if (y == 0)
                if (x > 0) ret.A = 0; else ret.A = 180;
            else
            {
                if (y >= 0 && x >= 0) ret.A = (int)(Math.Atan(y / x) * 180 / Math.PI);
                if (y >= 0 && x <= 0) ret.A = (int)(Math.Atan(y / -x) * 180 / Math.PI) + 90;
                if (y <= 0 && x <= 0) ret.A = (int)(Math.Atan(y / x) * 180 / Math.PI) + 180;
                if (y <= 0 && x >= 0) ret.A = (int)(Math.Atan(-y / x) * 180 / Math.PI) + 270;
            }

            return ret;
        }

        public static string toJson(object objeto, List<Type> knowntypes)
        {
            if (objeto == null)
                return "null";
            else
            {
                //http://www.esasp.net/2010/06/c-serializar-json-datacontract.html
                string s = string.Empty;
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(objeto.GetType(), knowntypes);
                MemoryStream ms = new MemoryStream();
                jsonSerializer.WriteObject(ms, objeto);
                s = Encoding.UTF8.GetString(ms.ToArray());
                return s;
            }
        }

        public static string HTMLLinksBR(string s)
        {
            //links con <a>
            s = s.Replace("\n", "<br>");

            List<string> links = new List<string>();
            for (int ini = 0; ini < s.Length; ini++)
            {
                string rest = s.Substring(ini);
                if (rest.StartsWith("http://") || rest.StartsWith("https://"))
                {
                    int fin = Math.Min(rest.IndexOf(" "), rest.IndexOf("<br>"));
                    if (fin == -1) fin = rest.Length; //fin de texto
                    links.Add(s.Substring(ini, fin));
                }
            }
            foreach(string lnk in links){
                s = s.Replace(lnk, "<a href='" + lnk + "' target='_blank'>" + lnk + "</a>");
            }
    
            return s;
        }

        public static string toJson(object objeto)
        {
            if (objeto == null)
                return "null";
            else
            {
                //http://www.esasp.net/2010/06/c-serializar-json-datacontract.html
                string s = string.Empty;
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(objeto.GetType());
                MemoryStream ms = new MemoryStream();
                jsonSerializer.WriteObject(ms, objeto);
                s = Encoding.UTF8.GetString(ms.ToArray());
                return s;
            }
        }

        public static DateTime getDateFromJSON(string value)
        {
            System.Text.RegularExpressions.Regex dateRegex = new System.Text.RegularExpressions.Regex(@"/Date\((\d+)([-+])(\d+)\)/");
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime d = DateTime.MinValue;
            System.Text.RegularExpressions.Match match = dateRegex.Match(value);
            if (match.Success)
            {
                // try to parse the string into a long. then create a datetime and convert to local time.
                long msFromEpoch;
                if (long.TryParse(match.Groups[1].Value, out msFromEpoch))
                {
                    TimeSpan fromEpoch = TimeSpan.FromMilliseconds(msFromEpoch);
                    d = TimeZoneInfo.ConvertTimeFromUtc(epoch.Add(fromEpoch), TimeZoneInfo.Local);
                }
            }
            return d;
        }

        public static T fromJson<T>(string jsonSerializado, List<Type> knowntypes)
        {
            try
            {
                //deserializo
                T obj = Activator.CreateInstance<T>();
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonSerializado));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType(), knowntypes);
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
                ms.Dispose();
                return obj;
            }
            catch (Exception ex) { throw new Exception("fromJson:" + ex.Message); }//return default(T); }
        }

        public static T fromJson<T>(string jsonSerializado)
        {
            try
            {
                //deserializo
                T obj = Activator.CreateInstance<T>();
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonSerializado));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
                ms.Dispose();
                return obj;
            }
            catch (Exception ex) { throw new Exception("fromJson:" + ex.Message); }//return default(T); }
        }

        public static PointF toRectDeg(Polar vector)
        {
            return toRectDeg(vector.M, vector.A);
        }

        public static PointF toRectDeg(float mod, float angDeg)
        {
            return new Point((int)(Math.Cos(angDeg * Math.PI / 180) * mod), (int)(Math.Sin(angDeg * Math.PI / 180) * mod));
        }

        public static PointF toRectRad(float mod, float angRad)
        {
            return new Point((int)(Math.Cos(angRad) * mod), (int)(Math.Sin(angRad) * mod));
        }

        public static string sendMailAlta(string grupo, string to, string nombre, string email, string basepath)
        {
            string msg = System.IO.File.ReadAllText(basepath + "\\alta.html");
            msg = msg.Replace("%1", nombre);
            msg = msg.Replace("%2", email);
            msg = msg.Replace("%3", grupo);

            return sendMail(to, "Solicitud de alta en [" + grupo + "]", msg);
        }

        public static string encolarMailCaido(string grupo, string to, string mailAdmin, string basepath)
        {
            string msg = System.IO.File.ReadAllText(basepath + "\\caido.html");
            msg = msg.Replace("%1", grupo);

            return encolarMail(to, "Flores caducadas", msg);
        }

        public static string sendMailWelcome(string grupo, string to, string clave, string url, string basepath)
        {
            string msg = System.IO.File.ReadAllText(basepath + "\\welcome.html");
            msg = msg.Replace("%1", url);
            msg = msg.Replace("%2", to);
            msg = msg.Replace("%3", clave);
            msg = msg.Replace("%4", grupo);

            return sendMail(to, "Alta Nabú", msg);
        }

        public static string sendMail(string to, string subject, string body)
        {
            //envio por mail
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            string SMTPURL = System.Configuration.ConfigurationManager.AppSettings["SMTPURL"];
            string from = System.Configuration.ConfigurationManager.AppSettings["SMTPFrom"];
            string user = System.Configuration.ConfigurationManager.AppSettings["SMTPUser"];
            string pass = System.Configuration.ConfigurationManager.AppSettings["SMTPPass"];
            string ret = "Enviado";

            if (SMTPURL != "")
            {
                try
                {
                    msg.From = new System.Net.Mail.MailAddress(from, from);
                    msg.Body = body;
                    msg.IsBodyHtml = true;
                    msg.Subject = subject;
                    msg.To.Add(new System.Net.Mail.MailAddress(to, to));

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(SMTPURL);
                    //smtp.EnableSsl = true;                                                    //esto en la CIC no funciona
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(user, pass);
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                    //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object s,  
                    //    System.Security.Cryptography.X509Certificates.X509Certificate certificate, 
                    //    System.Security.Cryptography.X509Certificates.X509Chain chain,
                    //    System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

                    smtp.Send(msg);
                }
                catch (Exception ex)
                {
                    ret = "Error=" + ex.Message;
                }
            }
            return ret;
        }

        public static string encolarMailInactivo(string to)
        {
            string msg = System.IO.File.ReadAllText(startupPath + "\\mails\\modelos\\ES\\inactivo.html");
            return encolarMail(to, "Inactivo en Nabu", msg);
        }

        public static string encolarMailNuevoConsenso(string to, int si, int no, string link)
        {
            string msg = System.IO.File.ReadAllText(startupPath + "\\mails\\modelos\\ES\\nuevoConsenso.html");
            msg = msg.Replace("%1", si.ToString());
            msg = msg.Replace("%2", no.ToString());
            msg = msg.Replace("%3", link);
            msg = msg.Replace("%4", link);

            return encolarMail(to, "Nuevo consenso", msg);
        }

        public static string encolarMail(string to, string subject, string body)
        {
            //envio por mail
            string msg = "";
            string ret = "Encolado";
            Random rnd = new Random();

            try
            {
                //obtengo nonmbre
                string name = startupPath + "\\mails\\cola\\" + ((int)(rnd.NextDouble() * 100000)).ToString() + ".txt";
                while (System.IO.File.Exists(name))
                    name = startupPath + "\\mails\\cola\\" + ((int)(rnd.NextDouble() * 100000)).ToString() + ".txt";

                msg += to + "\r\n";
                msg += subject + "\r\n";
                msg += body + "\r\n";

                System.IO.File.AppendAllText(name, msg);
            }
            catch (Exception ex)
            {
                ret = "Error=" + ex.Message;
            }
            return ret;
        }
    }
}
