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
        public static string errordebug = ""; //para depuracion

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
            "(Aparece en el pie del arbol)|es|(Aparece en el pie del &aacute;rbol)",
            "(Etiqueta en el arbol)|es|(Etiqueta en el &aacute;rbol)",
            "accion|es|Acci&oacute;n",
            "Accion finalizada [%1]|es|Acci&oacute;n finalizada [%1]",
            "accion.aquien.tip|es|Quienes se beneficiar&aacute;n con los resultados",
            "accion.aquien.titulo|es|A quien va dirigido",
            "accion.creada|es|Acci&oacute;n creada",
            "accion.descripcion.tip|es|Describe brevemente como sera la acci&oacute;n a realizar. Esfuerzos, plazos, costos, desplazamientos, implicaciones, fases.",
            "accion.descripcion.titulo|es|Descripci&oacute;n",
            "accion.error|es|Error",
            "accion.evaluacion.p1|es|&iquest;Crees que nuestra acción concreta ha facilitado el desarrollo de nuestros objetivos? Si no, &iquest;por qu&eacute;?",
            "accion.evaluacion.p2|es|&iquest;Hemos aprendido algo nuevo de la experiencia? Si es que si, &iquest;Qu&eacute; ha sido? Si es que no, &iquest;Qu&eacute; crees que ha frenado nuestro proceso de aprendizaje?",
            "accion.evaluacion.p3|es|&iquest;Te has sentido en seguridad, incluyente, participativo durante el proceso? si no, &iquest;Por qu&eacute;?",
            "accion.evaluacion.p4|es|&iquest;Las fases, costos, recursos y plazo han sido los adecuados? Si no, &iquest;Por qu&eacute;?",
            "accion.evaluacion.p5|es|&iquest;Crees que la persona que ha coordinado el proceso ha sido consecuente con su rol? Si no, &iquest;Por qu&eacute;?",
            "accion.evaluacion.p6|es|&iquest;Crees que se requiere otro ciclo de Acci&oacute;n para resolver este tema? Si es que si, &iquest;Por qu&eacute;?",
            "accion.evaluacion.tip1|es|Eval&uacute;a si la Acci&oacute;n era necesaria como se habia definido al tomar la decisi&oacute;n",
            "accion.evaluacion.tip2|es|Eval&uacute;a si el objetivo definido en la Acci&oacute;n finalmente se corresponde con el resultado obtenido",
            "accion.evaluacion.tip3|es|Eval&uacute;a si te has sentido en seguridad, y coparticipe del proceso ejecutivo",
            "accion.evaluacion.tip4|es|Eval&uacute;a si las fases, costos, recursos y plazo definidos al tomar la decis&oacute;n eran adecuados",
            "accion.evaluacion.tip5|es|Eval&uacute;a si la persona que ha coordinado el proceso, lo hizo adecuadamente",
            "accion.evaluacion.tip6|es|Eval&uacute;a si consideras necesario realizar otro ciclo de debate y Acci&oacute;n teniendo en cuenta el resultado obtenido",
            "accion.fases.tip|es|Describe las fases que se deben alcanzar para lograr el objetivo",
            "accion.fases.titulo|es|Fases",
            "accion.introduccion.tip|es|¿Cual es la situaci&oacute;n que requiere de esta propuesta de acci&oacute;n? ¿porqu&eacute; se debe actuar? ¿que mejorar&iacute;a? Ten en cuenta que la situati&oacute;n que describes represente al grupo",
            "accion.introduccion.titulo|es|Introducci&oacute;n",
            "accion.materiales.tip|es|Describe los recursos que ser&aacute;n necesarios sin olvidar un presupuesto estimado",
            "accion.materiales.titulo|es|Materiales",
            "accion.objetivo.tip|es|Describe que pretendes que logremos como resultado de realizar la acci&oacute;n propuesta. Motiva al grupo con este objetivo. ¿es un deseo del grupo lograrlo?",
            "accion.objetivo.titulo|es|Objetivo a lograr",
            "accion.presupuesto.tip|es|Estimaci&oacute;n de costos y plazos, el grupo debe conocer y aprobar estos valores",
            "accion.presupuesto.titulo|es|Presupuesto y plazo de entrega",
            "accion.responsable.tip|es|Responsable de proyecto",
            "accion.responsable.titulo|es|Responsable",
            "accion.revision.tip|es|Revisi&oacute;n",
            "accion.rrhh.tip|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado",
            "accion.rrhh.titulo|es|RRHH",
            "accion.software.tip|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado. Software, desplazamientos, servicios, etc.",
            "accion.software.titulo|es|Software",
            "Acta|es|Acta",
            "acta.apertura.tip|es|",
            "acta.apertura.titulo|es|Ronda de apertura",
            "acta.evaluacion.tip|es|",
            "acta.evaluacion.titulo|es|Evaluaci&oacute;n",
            "acta.logisticos.tip|es|",
            "acta.logisticos.titulo|es|Aspectos log&iacute;sticos",
            "acta.ordendeldia.tip|es|",
            "acta.ordendeldia.titulo|es|Orden del d&iacute;a",
            "Agregar tema|es|Agregar tema",
            "alhijo.evaluacion.p1|es|&iquest;El problema a tratar est&aacute; bien definido?",
            "alhijo.evaluacion.p2|es|&iquest;La situaci&oacute;n actual se corresponde con la realidad?",
            "alhijo.evaluacion.p3|es|&iquest;La propuesta deber&iacute;a analizarse?",
            "alhijo.evaluacion.p4|es|&iquest;La situaci&oacute;n deseada es asumible?",
            "alhijo.evaluacion.p5|es|&iquest;Este tema deber&iacute;a atenderse en este grupo?",
            "alhijo.evaluacion.tip1|es|",
            "alhijo.evaluacion.tip2|es|",
            "alhijo.evaluacion.tip3|es|",
            "alhijo.evaluacion.tip4|es|",
            "alhijo.evaluacion.tip5|es|",
            "alhijo.introduccion.tip|es|",
            "alhijo.introduccion.titulo|es|",
            "alhijo.propuesta.tip|es|",
            "alhijo.propuesta.titulo|es|Propuesta",
            "alhijo.situacionactual.tip|es|",
            "alhijo.situacionactual.titulo|es|Situaci&oacute;n actual",
            "alhijo.situaciondeseada.tip|es|",
            "alhijo.situaciondeseada.titulo|es|Situaci&oacute;n deseada",
            "alpadre.evaluacion.p1|es|&iquest;El problema a tratar est&aacute; bien definido?",
            "alpadre.evaluacion.p2|es|&iquest;La situaci&oacute;n actual se corresponde con la realidad?",
            "alpadre.evaluacion.p3|es|&iquest;La propuesta deber&iacute;a analizarse?",
            "alpadre.evaluacion.p4|es|&iquest;La situaci&oacute;n deseada es asumible?",
            "alpadre.evaluacion.p5|es|&iquest;Este tema deber&iacute;a atenderse en este grupo?",
            "alpadre.evaluacion.tip1|es|",
            "alpadre.evaluacion.tip2|es|",
            "alpadre.evaluacion.tip3|es|",
            "alpadre.evaluacion.tip4|es|",
            "alpadre.evaluacion.tip5|es|",
            "alpadre.introduccion.tip|es|",
            "alpadre.introduccion.titulo|es|Introducci&oacute;n",
            "alpadre.propuesta.tip|es|",
            "alpadre.propuesta.titulo|es|Propuesta",
            "alpadre.situacionactual.tip|es|",
            "alpadre.situacionactual.titulo|es|Situaci&oacute;n actual",
            "alpadre.situaciondeseada.tip|es|",
            "alpadre.situaciondeseada.tip|es|Situaci&oacute;n deseada",
            "alpadre.situaciondeseada.tip|es|",
            "alpadre.situaciondeseada.tip|es|Situaci&oacute;n deseada",
            "Aspectos logisticos|es|Aspectos log&iacute;sticos",
            "Autor de la publicacion|es|Autor de la publicaci&oacute;n",
            "Cancelar|es|Cancelar",
            "Cantidad de flores|es|Cantidad de flores debe estar entre 1 y 20",
            "Capacidaddes|es|Capacidades",
            "Cerrar|es|Cerrar",
            "Comentarios|es|Comentarios",
            "Comunicado al grupo padre|es|Comunicado al grupo padre",
            "Consecuencias|es|Consecuencias",
            "Consenso alcanzado|es|Decisi&oacute;n alcanzada",
            "Crea la propuesta|es|Crea la propuesta",
            "Crear evaluacion|es|Crear evaluaci&oacute;n",
            "Crear evaluacion|es|Crear evaluaci&oacute;n",
            "Crear evaluacion|es|Crear evaluaci&oacute;n",
            "Crear evaluacion|es|Crear evaluaci&oacute;n",
            "Crear nueva estrategia|es|Crear nueva estrategia",
            "Crear nuevo grupo de trabajo|es|Crear nuevo grupo de trabajo",
            "Crear propuesta|es|Crear propuesta",
            "Definir apertura|es|Definir apertura",
            "Definir evaluacion|es|Definir evaluaci&oacute;n",
            "Definir orden del dia|es|Definir orden del d&iacute;a",
            "Definir un titulo de Acta|es|Definir un t&iacute;tulo de Acta",
            "Documento de comunicado a evaluar|es|Documento de comunicado a evaluar",
            "Documento de resultado a evaluar|es|Documento de resultado a evaluar",
            "Documento simulado|es|Documento simulado",
            "El nodo no existe|es|El nodo no existe",
            "El objetivo no puede ser vacio|es|El objetivo no puede ser vac&iacute;o",
            "El titulo del manifiesto no puede ser vacio|es|El t&iacute;tulo del manifiesto no puede ser vac&iacute;o",
            "El usuario no existe|es|El usuario no existe",
            "El usuario no existe o no esta habilitado|es|El usuario no existe o no esta habilitado",
            "Eliminar estrategia|es|Eliminar estrategia",
            "Eliminar grupo de trabajo|es|Eliminar grupo de trabajo",
            "enlace.evaluacion.p1|es|&iquest;La tem&aacute;tica está muy relacionada con nuestros objetivos? Si es que si, &iquest;De qu&eacute; manera?",
            "enlace.evaluacion.p2|es|&iquest;Se ha tratado, o se está tratando esta tem&aacute;tica en el grupo?",
            "enlace.evaluacion.p3|es|&iquest;La tem&aacute;tica deber&iacute;a integrarse en nuestros objetivos? Si es que si, &iquest;Por qu&eacute;?",
            "enlace.evaluacion.p4|es|&iquest;La tem&aacute;tica nos obligar&iacute;a a revisar nuestros objetivos?",
            "enlace.evaluacion.p5|es|&iquest;Crees que este tema deber&iacute;a generar un nuevo ciclo de acci&oacute;n? Si es que si, &iquest;Por qu&eacute;?",
            "enlace.evaluacion.tip1|es|Eval&uacute;a si esta tem&aacute;tica esta en la linea de los intereses del grupo y puede o no aportar informaci&oacute;n",
            "enlace.evaluacion.tip2|es|Eval&uacute;a si esta tem&aacute;tica se puede considerar solida y veraz",
            "enlace.evaluacion.tip3|es|Eval&uacute;a si consideras que esta tem&aacute;tica facilita el alcance de nuestros objetivos",
            "enlace.evaluacion.tip4|es|Eval&uacute;a si consideras que esta tem&aacute;tica puede llegar a cambiar nuestros objetivos",
            "enlace.evaluacion.tip5|es|Eval&uacute;a si consideras que esta tem&aacute;tica deber&iacute;a provocar acciones reales en el grupo",
            "Enseña vista previa antes de proponer|es|Enseña vista previa antes de proponer",
            "Enviar|es|Enviar",
            "Este debate ya ha alcanzado el consenso|es|Este debate ya ha alcanzado la decisi&oacute;n",
            "Este manifiesto reemplaza al anterior|es|Este manifiesto reemplaza al anterior",
            "estrategia.actualizada|es|Estrategia actualizada en el grupo de trabajo [%1] en la estructura organizativa",
            "estrategia.creada|es|Estrategia creada en el grupo de trabajo [%1] en la estructura organizativa",
            "estrategia.eliminada|es|Estrategia eliminada del grupo de trabajo [%1] en la estructura organizativa",
            "Estructura organizativa actualizada|es|Estructura organizativa actualizada",
            "Etiqueta|es|Etiqueta",
            "Evaluacion caida para el tema [%1]|es|Evaluaci&oacute;n caida para el tema [%1]",
            "Evaluacion de Accion|es|Evaluaci&oacute;n de Acci&oacute;n",
            "Evaluación de Accion|es|Evaluaci&oacute;n de Acci&oacute;n",
            "Evaluacion de comunicado intergrupal|es|Evaluaci&oacute;n de comunicado intergrupal",
            "Evaluacion de Enlace|es|Evaluaci&oacute;n de Enlace",
            "Evaluacion de Evento|es|Evaluaci&oacute;n de Evento",
            "evento.aquien.tip|es|A quienes va dirigido el evento y quienes se beneficiaran con los resultados",
            "evento.aquien.titulo|es|A quien va dirigido",
            "evento.creado|es|Evento creado",
            "evento.descripcion.tip|es|Describe como ser&aacute; el evento. ¿show? ¿presentaciones? ¿ponentes? ¿musica? ¿actividades? ¿payasos? ¿catering?",
            "evento.descripcion.titulo|es|Descripci&oacute;n",
            "evento.error|es|Error",
            "evento.evaluacion.p1|es|&iquest;Crees que este evento ha facilitado el desarrollo de nuestros objetivos? Si no, ¿por qu&eacute;?",
            "evento.evaluacion.p2|es|&iquest;Hemos aprendido algo nuevo de la experiencia? Si es que si, qu&eacute; ha sido? Si es que no, ¿Qu&eacute; crees que ha frenado nuestro proceso de aprendizaje?",
            "evento.evaluacion.p3|es|&iquest;Te has sentido en seguridad, incluyente, coparticipe durante el proceso? si no, ¿Por qu&eacute;?",
            "evento.evaluacion.p4|es|&iquest;Las fases, costos, recursos y plazo han sido los adecuados? Si no, ¿por qu&eacute;?",
            "evento.evaluacion.p5|es|&iquest;Crees que la persona que ha coordinado el proceso ha sido consecuente con su rol? Si no, ¿por qu&eacute;?",
            "evento.evaluacion.p6|es|&iquest;Crees que el resultado de esta evaluaci&oacute;n debe generar un ajuste de nuestros objetivos? Si es que si, ¿Por qu&eacute;",
            "evento.evaluacion.tip1|es|Eval&uacute;a si el Evento era necesario como se habia definido al tomar la decisi&oacute;n",
            "evento.evaluacion.tip2|es|Eval&uacute;a si el objetivo definido para el Evento finalmente se corresponde con el resultado obtenido",
            "evento.evaluacion.tip3|es|Eval&uacute;a si te has sentido en seguridad, y coparticipe del proceso ejecutivo",
            "evento.evaluacion.tip4|es|Eval&uacute;a si las fases, costos, recursos y plazo definidos al tomar la decis&oacute;n eran adecuadas",
            "evento.evaluacion.tip5|es|Eval&uacute;a si la persona que ha coordinado el proceso, lo hizo adecuadamente",
            "evento.evaluacion.tip6|es|Eval&uacute;a si consideras necesario realizar otro ciclo de debate y acci&oacute;n teniendo en cuenta el resultado obtenido",
            "evento.fecha.tip|es|Haz una propuesta de fecha para el evento",
            "evento.fecha.titulo|es|Haz una propuesta de fecha para el evento",
            "evento.introduccion.tip|es|¿Cual es la situaci&oacute;n que requiere de esta propuesta de evento? ¿porqu&eacute; se debe hacer? ¿cual es la motivaci&oacute;n? Ten en cuenta que la situati&oacute;n que describes represente al grupo",
            "evento.introduccion.titulo|es|Introducci&oacute;n",
            "evento.lugar.tip|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado",
            "evento.lugar.titulo|es|Lugar",
            "evento.materiales.tip|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado",
            "evento.materiales.titulo|es|Materiales",
            "evento.objetivo.tip|es|Describe que pretendes que logremos realizando este evento. Motiva al grupo con este objetivo. ¿es un deseo del grupo lograrlo?",
            "evento.objetivo.titulo|es|Objetivo a lograr",
            "evento.organizacion.tip|es|Describe como se debe organizar este evento, pasos a seguir, tiempos, desplazamientos, contrataciones, etc.",
            "evento.organizacion.titulo|es|Organizaci&oacute;n",
            "evento.responsable.tip|es|",
            "evento.responsable.titulo|es|Responsable",
            "evento.transportes.tip|es|Describe los recursos que seran necesarios sin olvidar un presupuesto estimado. Software, desplazamientos, servicios, etc.",
            "evento.transportes.titulo|es|Transportes",
            "evento.valoracion.tip|es|&iquest;C&oacute;mo se medir&aacute; el resultado de este evento? ¿entradas vendidas? ¿cantidad de personas presentes? ¿reconocimiento alcanzado? ¿efectos sociales? ¿publicdad?. <br>La valoraci&oacute;n del evento debe ser cuantificable y facil de comprender. Debe quedar claro si ha ido bien o no.",
            "evento.valoracion.titulo|es|Valoraci&oacute;n del resultado",
            "Fecha|es|Fecha",
            "Fecha|es|Fecha",
            "Fecha|es|Fecha",
            "Fecha|es|Fecha",
            "Fecha celebracion|es|Fecha celebraci&oacute;n",
            "Fecha celebracion|es|Fecha celebraci&oacute;n",
            "Fecha celebracion|es|Fecha celebraci&oacute;n",
            "Fecha celebracion|es|Fecha celebraci&oacute;n",
            "Fecha proxima celebracion|es|Fecha pr&oacute;xima celebraci&oacute;n",
            "Fecha proxima celebracion|es|Fecha pr&oacute;xima celebraci&oacute;n",
            "Fecha proxima celebracion|es|Fecha pr&oacute;xima celebraci&oacute;n",
            "Fecha proxima celebracion|es|Fecha pr&oacute;xima celebraci&oacute;n",
            "Grupo hijo borrado|es|Grupo hijo borrado",
            "grupoTrabajo.actualizado.tip|es|Grupo de trabajo actualizado en la estructura organizativa",
            "grupoTrabajo.actualizado.titulo|es|Grupo de trabajo actualizado en la estructura organizativa",
            "grupoTrabajo.aquien.tip|es|Quienes se beneficiaran con los resultados",
            "grupoTrabajo.aquien.titulo|es|A quien va dirigido",
            "grupoTrabajo.capacidades.tip|es|&iquest;Qu&eacute; capacidades deben tener las personas que componen el nuevo grupo? Conocimientos, experiencias, etc.",
            "grupoTrabajo.capacidades.titulo|es|Capacidades necesarias",
            "grupoTrabajo.conclusiones.tip|es|&iquest;Qu&eacute; hemos aprendido con esta experiencia?",
            "grupoTrabajo.conclusiones.titulo|es|Conclusiones",
            "grupoTrabajo.consecuencias.tip|es|&iquest;Cuales son los consecuencias de eliminar este grupo de trabajo que habr&aacute; que enfrentar?",
            "grupoTrabajo.consecuencias.titulo|es|Consecuencias",
            "grupoTrabajo.creado.tip|es|Grupo de trabajo creado en la estructura organizativa",
            "grupoTrabajo.creado.titulo|es|Grupo de trabajo creado en la estructura organizativa",
            "grupoTrabajo.descripcion.tip|es|Describe brevemente las actividades a realizar para alcanzar los objetivos.",
            "grupoTrabajo.descripcion.titulo|es|Descripci&oacute;n",
            "grupoTrabajo.eliminado.tip|es|Grupo de trabajo eliminado de la estructura organizativa",
            "grupoTrabajo.eliminado.titulo|es|Grupo de trabajo eliminado de la estructura organizativa",
            "grupoTrabajo.integrantes.tip|es|&iquest;Quienes ser&aacute;n los integrantes del nuevo grupo de trabajo? Estos nombres sirven de propuesta o referencia. No implican realmente a las personas. Si no quieres implicar nombres reales di 'a definir'.",
            "grupoTrabajo.integrantes.titulo|es|Integrantes",
            "grupoTrabajo.introduccion.tip|es|&iquest;Cual es la sicuaci&oacute;n actual?, ¿Por qu&eacute; necesitamos un nuevo grupo de trabajo?, ¿Qu&eacute; problemas resolver&aacute;? Ten en cuenta que la situati&oacute;n que describes represente al grupo",
            "grupoTrabajo.introduccion.titulo|es|Introducci&oacute;n",
            "grupoTrabajo.objetivo.tip|es|&iquest;Cuales son los objetivos del nuevo grupo de trabajo?",
            "grupoTrabajo.objetivo.titulo|es|Objetivo a lograr",
            "grupoTrabajo.recursos.tip|es|&iquest;Qu&eacute; recursos ser&aacute;n necesarios para el nuevo grupo de trabajo? Mobiliario, ordenadores, sitio de trabajo, etc.",
            "grupoTrabajo.recursos.titulo|es|Recursos",
            "grupoTrabajo.revision.tip|es|&iquest;Con qu&eacute; periodicidad deben evaluarse los resultados alcanzados.",
            "grupoTrabajo.revision.titulo|es|Revisi&oacute;n de valoraci&oacute;n del resultado",
            "grupoTrabajo.valoracion.tip|es|&iquest;C&oacute;mo se medir&aacute; el resultado de este grupo de trabajo? ¿cantidad de procesos ejecutados? ¿m&eacute;tricas de eficiancia? ¿reconocimiento de otras personas? ¿resultados concretos?. <br>La valoraci&oacute;n del desempeño del grupo debe ser cuantificable y facil de comprender. Debe quedar claro para todos si cumple con su objetivo o no.",
            "grupoTrabajo.valoracion.titulo|es|Valoraci&oacute;n del resultado",
            "Integrantes|es|Integrantes",
            "La clave actual no corresponde|es|La clave actual no corresponde",
            "La clave nueva no puede ser vacia|es|La clave nueva no puede ser vacia",
            "La evaluacion anterior ha sido reemplazada|es|La evaluaci&oacute;n<br> anterior ha sido<br> reemplazada",
            "La mision no ser vacia|es|La misi&oacute;n no puede ser vac&iacute;a",
            "La vision no puede ser vacia|es|La visi&oacute;n no puede ser vac&iacute;a",
            "Lugar|es|Lugar",
            "Manifiesto|es|Manifiesto",
            "Manifiesto actualizado en el grupo|es|Manifiesto actualizado en el grupo",
            "manifiesto.mision.tip|es|La misi&oacute;n define cual es nuestra labor, ¿qu&eacute; hacemos?, ¿a qu&eacute; nos dedicamos?, ¿en qu&eacute; sector social sucede?, ¿qui&eacute;nes son nuestro p&uacute;blico objetivo?, ¿cu&aacute;l es nuestro &aacute;mbito geogr&aacute;fico de acci&oacute;n?",
            "manifiesto.mision.tip|es|Misi&oacute;n",
            "manifiesto.mision.tip|es|La misi&oacute;n define cual es nuestra labor, ¿qu&eacute; hacemos?, ¿a qu&eacute; nos dedicamos?, ¿en que sector social sucede?, ¿qui&eacute;nes son nuestro p&uacute;blico objetivo?, ¿cu&aacute;l es nuestro &aacute;mbito geogr&aacute;fico de acci&oacute;n?",
            "manifiesto.mision.tip|es|Misi&oacute;n",
            "manifiesto.objetivos.tip|es|Los objetivos concretan la misi&oacute;n, nos dan un sentido de temporalidad. Deben hacer aparecer la relaci&oacute;n de beneficio mutuo entre la organizaci&oacute;n y su entorno.",
            "manifiesto.objetivos.titulo|es|Objetivos",
            "manifiesto.servicios.tip|es|&iquest;Qu&eacute; servicios prestamos para alcanzar nuestra misi&oacute;n?. Estos servicios son el punto partida para la creaci&oacute;n de procesos operativos y grupos de trabajo. Lista los servicios con nombre y una breve descripci&oacute;n",
            "manifiesto.servicios.titulo|es|Servicios",
            "manifiesto.vision.tip|es|La visi&oacute;n tiene un car&aacute;cter inspirador y motivador para el grupo. ¿Que visi&oacute;n tenemos del mundo?, ¿C&oacute;mo deber&iacute;an ser las cosas?",
            "max|es|M&aacute;x",
            "Maximos usuarios negados|es|M&aacute;ximos usuarios negados debe estar entre 0 y 50",
            "Minimos usuarios implicados|es|M&iacute;nimos usuarios implicados debe estar entre 50 y 100",
            "Modificar estrategia|es|Modificar estrategia",
            "Modificar grupo de trabajo|es|Modificar grupo de trabajo",
            "Nivel [%1] no existe en este modelo|es|Nivel [%1] no existe en este modelo",
            "Nivel en el arbol|es|Nivel en el &aacute;rbol",
            "No hay grupos hijos en este grupo|es|No hay grupos hijos en este grupo",
            "No tiene flores para crear una propuesta|es|No tiene flores para crear una propuesta",
            "Nombre|es|Nombre",
            "Nombre de la publicacion|es|Nombre de la publicaci&oacute;n",
            "Nombre de nodo no puede ser vacio|es|Nombre de nodo no puede ser vacio",
            "Nueva Acta publicada|es|Nueva Acta publicada",
            "Nueva decision [%1]|es|[%1] Decisi&oacute;n alcanzada",
            "Nuevo grupo hijo agregado|es|Nuevo grupo hijo agregado",
            "Organizacion|es|Organizaci&oacute;n",
            "Palabras clave|es|Palabras clave",
            "Participantes|es|Participantes",
            "Permite corregir errores|es|Permite corregir errores",
            "Prevista de evaluacion|es|Prevista de evaluaci&oacute;n",
            "Prevista de evaluacion|es|Prevista de evaluaci&oacute;n",
            "Prevista de evaluacion|es|Prevista de evaluaci&oacute;n",
            "Prevista de evaluacion|es|Prevista de evaluaci&oacute;n",
            "Prevista de propuesta|es|Prevista de propuesta",
            "proceso.aquien.tip|es|Quienes se beneficiaran con los resultados de utilizar esta estrategia",
            "proceso.aquien.titulo|es|A quien va dirigdo",
            "proceso.conclusiones.tip|es|¿Que hemos aprendido con esta experiencia?",
            "proceso.conclusiones.titulo|es|Conclusiones",
            "proceso.consecuancias.tip|es|¿Cuales son los consecuencias de eliminar este grupo de trabajo que habr&aacute; que enfrentar?",
            "proceso.consecuancias.titulo|es|¿Cuales son los consecuencias de eliminar este grupo de trabajo que habr&aacute; que enfrentar?",
            "proceso.definicion.tip|es|Define paso por paso las tareas a realizar cuando se ejecute este proceso. Define como se tratar&aacute;n las excepciones. Una estrategia es un flujo de condiciones y tareas que contempla todas las alternativas posibles de manera que su ejecuci&oacute;n no admita discusiones sobre sus resultados. Un proceso operativo bien definido es parte importante de la definici&oacute;n operativa del grupo. Todas las actividades habituales del grupo deben definirse como procesos operativos.",
            "proceso.definicion.titulo|es|Definici&oacute;n de la estrategia",
            "proceso.descripcion.tip|es|Describe brevemente los pasos a realizar para usar esta estrategia. Menciona como se tratar&aacute;n los casos excepcionales. Una estrategia siempre debe generar un resultado, aunque sea un registro de fallo para su posterior an&aacute;lisis.",
            "proceso.descripcion.titulo|es|Descripci&oacute;n",
            "proceso.entradas.tip|es|¿C&oacute;mo se provoca la ejecuci&oacute;n de este proceso? ¿Qu&eacute; necesita este proceso para poder ejecutarse? Un nuevo socio, un documento, una peticion personal, etc.",
            "proceso.entradas.titulo|es|Entradas de proceso",
            "proceso.grupo.tip|es|¿Qu&eacute; grupo de trabajo usa esta estrategia?",
            "proceso.grupo.titulo|es|Grupo de trabajo",
            "proceso.implantacion.tip|es|¿Qu&eacute; se debe hacer para implantar este proceso? Recursos, sitio fis&iacute;co, conocimientos y capacidades de las personas, formularios/hojas de datos/folletos informativos, software, etc.",
            "proceso.implantacion.titulo|es|Implantaci&oacute;n",
            "proceso.introduccion.tip|es|¿Cual es la sicuaci&oacute;n actual?, ¿Por qu&eacute; necesitamos una nueva estrategia?, ¿Qu&eacute; problemas resolver&aacute;? Ten en cuenta que la situati&oacute;n que describes represente un problema real del grupo",
            "proceso.introduccion.titulo|es|¿Cual es la sicuaci&oacute;n actual?, ¿Por qu&eacute; necesitamos una nueva estrategia?, ¿Qu&eacute; problemas resolver&aacute;? Ten en cuenta que la situati&oacute;n que describes represente un problema real del grupo",
            "proceso.objetivo|es|¿Cuales son los objetivos del nuevo grupo de trabajo?",
            "proceso.objetivo.tip|es|",
            "proceso.objetivo.titulo|es|Objetivo",
            "proceso.revision.tip|es|¿Cuando se revisar&aacute; la definici&oacute;n de esta estrategia?",
            "proceso.revision.titulo|es|Revisi&oacute;n de valoraci&oacute;n del resultado",
            "proceso.valoracion.tip|es|¿C&oacute;mo se medir&aacute; el resultado de esta estrategia? ¿cantidad de procesos ejecutados? ¿resultados obtenidos? ¿facilita la necesidad? ¿es facil de ejecutar?. <br>La valoraci&oacute;n de un proceso operativo debe ser cuantificable y facil de comprender. Debe quedar claro para todos si cumple con su objetivo o no.",
            "proceso.valoracion.titulo|es|Valoraci&oacute;n del resultado",
            "Proponer variante|es|Proponer variante",
            "Recursos|es|Recursos",
            "Representante|es|Representante",
            "Representantes|es|Representantes",
            "Revisar evaluacion|es|Revisar evaluaci&oacute;n",
            "Revisar evaluacion|es|Revisar evaluaci&oacute;n",
            "Revisar evaluacion|es|Revisar evaluaci&oacute;n",
            "Revisar evaluacion|es|Revisar evaluaci&oacute;n",
            "Revisar propuesta|es|Revisar propuesta",
            "Se deben definir los participantes|es|Se deben definir los participantes",
            "Seleccione un nodo|es|Seleccione un nodo",
            "Simulacion|es|Simulaci&oacute;n!!!",
            "tema|es|Tema",
            "Titulo|es|T&iacute;tulo",
            "URL de la publicacion|es|URL de la publicaci&oacute;n",
            "Usuario [%1] actualizado|es|Usuario [%1] actualizado",
            "Usuario [%1] borrado|es|Usuario [%1] borrado",
            "Usuario invalido|es|Usuario inv&aacute;lido, operaci&oacute;n registrada!",
            "Usuario no habilitado|es|Usuario no habilitado para el grupo [%1]",
            "Usuario o clave incorrectos|es|Usuario o clave incorrectos para el grupo [%1]",
            "Usuarios creados desahibilitados|es|Usuarios creados en [%1] desahibilitados, contacte al coordinador para habilitarlos",
            "Vision|es|Visi&oacute;n",

            "(Aparece en el pie del arbol)|ct|(Apareix al peu de l'arbre)",
            "(Etiqueta en el arbol)|ct|(Etiqueta a l'arbre)",
            "accion|ct|Acci&oacute;",
            "Accion finalizada [%1]|ct|?",
            "accion.aquien.tip|ct|Qui es beneficiar&agrave; amb els resultats",
            "accion.aquien.titulo|ct|A qui va dirigit",
            "accion.creada|ct|?",
            "accion.descripcion.tip|ct|Descriu breument com ser&agrave; l'acci&oacute; a realitzar. Esforços, terminis, costos, desplaçaments, implicacions, fases.",
            "accion.descripcion.titulo|ct|Descripci&oacute;",
            "accion.error|ct|?",
            "accion.evaluacion.p1|ct|?",
            "accion.evaluacion.p2|ct|?",
            "accion.evaluacion.p3|ct|?",
            "accion.evaluacion.p4|ct|?",
            "accion.evaluacion.p5|ct|?",
            "accion.evaluacion.p6|ct|?",
            "accion.evaluacion.tip1|ct|?",
            "accion.evaluacion.tip2|ct|?",
            "accion.evaluacion.tip3|ct|?",
            "accion.evaluacion.tip4|ct|?",
            "accion.evaluacion.tip5|ct|?",
            "accion.evaluacion.tip6|ct|?",
            "accion.fases.tip|ct|Descriu les fases que s'han de seguir per a aconseguir l'objectiu",
            "accion.fases.titulo|ct|Fases",
            "accion.introduccion.tip|ct|Quina &eacute;s la situaci&oacute; que requereix d'aquesta proposta d'acci&oacute;? Per qu&egrave; s'ha d'actuar? Qu&egrave; millorar&agrave;? Assegura't que la situaci&oacute; que descrius representi el grup",
            "accion.introduccion.titulo|ct|Introducci&oacute;",
            "accion.materiales.tip|ct|Descriu els recursos que seran necessaris sense oblidar un pressupost estimat",
            "accion.materiales.titulo|ct|Materials",
            "accion.objetivo.tip|ct|Descriu qu&egrave; vols que aconseguim com a resultat de realitzar l'acci&oacute; proposada. Motiva el grup amb aquest objectiu. &Eacute;s un desig del grup aconseguir-ho?",
            "accion.objetivo.titulo|ct|Objectiu a aconseguir",
            "accion.presupuesto.tip|ct|Estimaci&oacute; de costos i terminis. El grup ha de con&egrave;ixer i aprovar aquests valors.",
            "accion.presupuesto.titulo|ct|Pressupost i termini d'entrega",
            "accion.responsable.tip|ct|?",
            "accion.responsable.titulo|ct|?",
            "accion.revision.tip|ct|?",
            "accion.rrhh.tip|ct|Descriu els recursos humans que seran necessaris sense oblidar un pressupost estimat",
            "accion.rrhh.titulo|ct|RRHH",
            "accion.software.tip|ct|Descriu els recursos de programari que seran necessaris sense oblidar un pressupost estimat. Programari, serveis, etc.",
            "accion.software.titulo|ct|Programari",
            "Acta|ct|?",
            "acta.apertura.tip|ct|?",
            "acta.apertura.titulo|ct|?",
            "acta.evaluacion.tip|ct|?",
            "acta.evaluacion.titulo|ct|?",
            "acta.logisticos.tip|ct|?",
            "acta.logisticos.titulo|ct|?",
            "acta.ordendeldia.tip|ct|?",
            "acta.ordendeldia.titulo|ct|?",
            "Agregar tema|ct|?",
            "alhijo.evaluacion.p1|ct|?",
            "alhijo.evaluacion.p2|ct|?",
            "alhijo.evaluacion.p3|ct|?",
            "alhijo.evaluacion.p4|ct|?",
            "alhijo.evaluacion.p5|ct|?",
            "alhijo.evaluacion.tip1|ct|?",
            "alhijo.evaluacion.tip2|ct|?",
            "alhijo.evaluacion.tip3|ct|?",
            "alhijo.evaluacion.tip4|ct|?",
            "alhijo.evaluacion.tip5|ct|?",
            "alhijo.introduccion.tip|ct|?",
            "alhijo.introduccion.titulo|ct|?",
            "alhijo.propuesta.tip|ct|?",
            "alhijo.propuesta.titulo|ct|?",
            "alhijo.situacionactual.tip|ct|?",
            "alhijo.situacionactual.titulo|ct|?",
            "alhijo.situaciondeseada.tip|ct|?",
            "alhijo.situaciondeseada.titulo|ct|?",
            "alpadre.evaluacion.p1|ct|?",
            "alpadre.evaluacion.p2|ct|?",
            "alpadre.evaluacion.p3|ct|?",
            "alpadre.evaluacion.p4|ct|?",
            "alpadre.evaluacion.p5|ct|?",
            "alpadre.evaluacion.tip1|ct|?",
            "alpadre.evaluacion.tip2|ct|?",
            "alpadre.evaluacion.tip3|ct|?",
            "alpadre.evaluacion.tip4|ct|?",
            "alpadre.evaluacion.tip5|ct|?",
            "alpadre.introduccion.tip|ct|?",
            "alpadre.introduccion.titulo|ct|?",
            "alpadre.propuesta.tip|ct|?",
            "alpadre.propuesta.titulo|ct|?",
            "alpadre.situacionactual.tip|ct|?",
            "alpadre.situacionactual.titulo|ct|?",
            "alpadre.situaciondeseada.tip|ct|?",
            "alpadre.situaciondeseada.tip|ct|?",
            "Aspectos logisticos|ct|?",
            "Autor de la publicacion|ct|?",
            "Cancelar|ct|Cancel&middot;lar",
            "Cantidad de flores|ct|La Quantitat de flors ha d'estar entre 1 i 20",
            "Capacidaddes|ct|?",
            "Cerrar|ct|Tancar",
            "Comentarios|ct|Comentaris",
            "Comunicado al grupo padre|ct|?",
            "Consecuencias|ct|?",
            "Consenso alcanzado|ct|Consens aconseguit",
            "Crea la propuesta|ct|Crea la proposta",
            "Crear evaluacion|ct|?",
            "Crear evaluacion|ct|?",
            "Crear nueva estrategia|ct|Crear nou estrategia",
            "Crear nuevo grupo de trabajo|ct|Crear nou grup de treball",
            "Crear propuesta|ct|Crear proposta",
            "Definir apertura|ct|?",
            "Definir evaluacion|ct|?",
            "Definir orden del dia|ct|?",
            "Definir un titulo de Acta|ct|?",
            "Documento de comunicado a evaluar|ct|?",
            "Documento de resultado a evaluar|ct|?",
            "Documento simulado|ct|Document simulat",
            "El nodo no existe|ct|Aquest node encara no existeix",
            "El objetivo no puede ser vacio|ct|L'objectiu no pot estar buit",
            "El titulo del manifiesto no puede ser vacio|ct|El t&iacute;tol del manifest no pot estar buit",
            "El usuario no existe|ct|La usu&agrave;ria no existeix",
            "El usuario no existe o no esta habilitado|ct|L'usuari no existeix o no aquesta habilitat",
            "Eliminar estrategia|ct|Eliminar estrategia",
            "Eliminar grupo de trabajo|ct|Eliminar grup de treball",
            "enlace.evaluacion.p1|ct|?",
            "enlace.evaluacion.p2|ct|?",
            "enlace.evaluacion.p3|ct|?",
            "enlace.evaluacion.p4|ct|?",
            "enlace.evaluacion.p5|ct|?",
            "enlace.evaluacion.tip1|ct|?",
            "enlace.evaluacion.tip2|ct|?",
            "enlace.evaluacion.tip3|ct|?",
            "enlace.evaluacion.tip4|ct|?",
            "enlace.evaluacion.tip5|ct|?",
            "Enseña vista previa antes de proponer|ct|Ensenya vista pr&egrave;via abans de proposar",
            "Enviar|ct|Enviar",
            "Este debate ya ha alcanzado el consenso|ct|Aquest debat ja ha arribat al consens",
            "Este manifiesto reemplaza al anterior|ct|Aquest manifest reemplaça l'anterior",
            "estrategia.actualizada|ct|Estrategia actualitzat al grup de treball [%1] a l'estructura organitzativa",
            "estrategia.creada|ct|Estrategia creat al grup de treball [%1] a l'estructura organitzativa",
            "estrategia.eliminada|ct|Estrategia eliminat del grup de treball [%1] a l'estructura organitzativa",
            "Estructura organizativa actualizada|ct|Estructura organitzativa actualitzada",
            "Etiqueta|ct|Etiqueta",
            "Evaluacion caida para el tema [%1]|ct|?",
            "Evaluacion de Accion|ct|?",
            "Evaluación de Accion|ct|?",
            "Evaluacion de comunicado intergrupal|ct|?",
            "Evaluacion de Enlace|ct|?",
            "Evaluacion de Evento|ct|?",
            "evento.aquien.tip|ct|?",
            "evento.aquien.titulo|ct|?",
            "evento.creado|ct|?",
            "evento.descripcion.tip|ct|?",
            "evento.descripcion.titulo|ct|?",
            "evento.error|ct|?",
            "evento.evaluacion.p1|ct|?",
            "evento.evaluacion.p2|ct|?",
            "evento.evaluacion.p3|ct|?",
            "evento.evaluacion.p4|ct|?",
            "evento.evaluacion.p5|ct|?",
            "evento.evaluacion.p6|ct|?",
            "evento.evaluacion.tip1|ct|?",
            "evento.evaluacion.tip2|ct|?",
            "evento.evaluacion.tip3|ct|?",
            "evento.evaluacion.tip4|ct|?",
            "evento.evaluacion.tip5|ct|?",
            "evento.evaluacion.tip6|ct|?",
            "evento.fecha.tip|ct|?",
            "evento.fecha.titulo|ct|?",
            "evento.introduccion.tip|ct|?",
            "evento.introduccion.titulo|ct|?",
            "evento.lugar.tip|ct|?",
            "evento.lugar.titulo|ct|?",
            "evento.materiales.tip|ct|?",
            "evento.materiales.titulo|ct|?",
            "evento.objetivo.tip|ct|?",
            "evento.objetivo.titulo|ct|?",
            "evento.organizacion.tip|ct|?",
            "evento.organizacion.titulo|ct|?",
            "evento.responsable.tip|ct|?",
            "evento.responsable.titulo|ct|?",
            "evento.transportes.tip|ct|?",
            "evento.transportes.titulo|ct|?",
            "evento.valoracion.tip|ct|?",
            "evento.valoracion.titulo|ct|?",
            "Fecha|ct|Data",
            "Fecha|ct|Data",
            "Fecha celebracion|ct|?",
            "Fecha celebracion|ct|?",
            "Fecha proxima celebracion|ct|?",
            "Fecha proxima celebracion|ct|?",
            "Grupo hijo borrado|ct|Grup filla borrat",
            "grupoTrabajo.actualizado.tip|ct|?",
            "grupoTrabajo.actualizado.titulo|ct|?",
            "grupoTrabajo.aquien.tip|ct|?",
            "grupoTrabajo.aquien.titulo|ct|?",
            "grupoTrabajo.capacidades.tip|ct|?",
            "grupoTrabajo.capacidades.titulo|ct|?",
            "grupoTrabajo.conclusiones.tip|ct|?",
            "grupoTrabajo.conclusiones.titulo|ct|?",
            "grupoTrabajo.consecuencias.tip|ct|?",
            "grupoTrabajo.consecuencias.titulo|ct|?",
            "grupoTrabajo.creado.tip|ct|?",
            "grupoTrabajo.creado.titulo|ct|?",
            "grupoTrabajo.descripcion.tip|ct|?",
            "grupoTrabajo.descripcion.titulo|ct|?",
            "grupoTrabajo.eliminado.tip|ct|?",
            "grupoTrabajo.eliminado.titulo|ct|?",
            "grupoTrabajo.integrantes.tip|ct|?",
            "grupoTrabajo.integrantes.titulo|ct|?",
            "grupoTrabajo.introduccion.tip|ct|?",
            "grupoTrabajo.introduccion.titulo|ct|?",
            "grupoTrabajo.objetivo.tip|ct|?",
            "grupoTrabajo.objetivo.titulo|ct|?",
            "grupoTrabajo.recursos.tip|ct|?",
            "grupoTrabajo.recursos.titulo|ct|?",
            "grupoTrabajo.revision.tip|ct|?",
            "grupoTrabajo.revision.titulo|ct|?",
            "grupoTrabajo.valoracion.tip|ct|?",
            "grupoTrabajo.valoracion.titulo|ct|?",
            "Integrantes|ct|?",
            "La clave actual no corresponde|ct|La clau actual no correspon",
            "La clave nueva no puede ser vacia|ct|La clau nova no pot estar buida",
            "La evaluacion anterior ha sido reemplazada|ct|?",
            "La mision no ser vacia|ct|La missi&oacutre; no pot estar buida",
            "La vision no puede ser vacia|ct|La visi&oacute; no pot estar buida",
            "Lugar|ct|?",
            "Manifiesto|ct|Manifest",
            "Manifiesto actualizado en el grupo|ct|Manifest actualizat al grup",
            "manifiesto.mision.tip|ct|La missi&oacute; defineix quina &eacute;s la nostra feina, qu&egrave; fem?, a qu&egrave; ens dediquem?, en quin sector social succeeix?, qui &eacute;s el nostre p&uacute;blic objectiu?, quin &eacute;s el nostre &agrave;mbit geogr&agrave;fic d'actuaci&oacute;?",
            "manifiesto.mision.tip|ct|Missi&oacute;",
            "manifiesto.mision.tip|ct|La missi&oacute; defineix quina &eacute;s la nostra feina, qu&egrave; fem?, a qu&egrave; ens dediquem?, en quin sector social succeeix?, qui &eacute;s el nostre p&uacute;blic objectiu?, quin &eacute;s el nostre &agrave;mbit geogr&agrave;fic d'actuaci&oacute;?",
            "manifiesto.mision.tip|ct|Missi&oacute;",
            "manifiesto.objetivos.tip|ct|?",
            "manifiesto.objetivos.titulo|ct|?",
            "manifiesto.servicios.tip|ct|Quins serveis prestem per a aconseguir la nostra missi&oacute;? Aquests serveis s&oacute;n el punt de partida per a la creaci&oacute; de processos operatius i grups de treball. Llista els serveis amb nom i una breu descripci&oacute;.",
            "manifiesto.servicios.titulo|ct|Serveis",
            "manifiesto.servicios.titulo|ct|Serveis",
            "manifiesto.vision.tip|ct|La visi&oacute; t&eacute; un car&agrave;cter inspirador i motivador per al grup. Quina visi&oacute; tenim del m&oacute;n?, Com haurien de ser, les coses?",
            "max|ct|Màx",
            "Maximos usuarios negados|ct|El m&agrave;xim d'usu&agrave;ries negades ha d'estar entre 0 i 50",
            "Minimos usuarios implicados|ct|El m&iacute;nim d'usu&agrave;ries implicades ha d'estar entre 50 i 100",
            "Modificar estrategia|ct|Modificar estrategia",
            "Modificar grupo de trabajo|ct|Modificar grup de treball",
            "Nivel [%1] no existe en este modelo|ct|El nivell [%1] no existeix en aquest model",
            "Nivel en el arbol|ct|Nivell a l'arbre",
            "No hay grupos hijos en este grupo|ct|?",
            "No tiene flores para crear una propuesta|ct|No tens flors per a crear una proposta",
            "Nombre|ct|Nom",
            "Nombre de la publicacion|ct|?",
            "Nombre de nodo no puede ser vacio|ct|Nom de node no pot ser buit",
            "Nueva Acta publicada|ct|?",
            "Nueva decision [%1]|ct|?",
            "Nuevo grupo hijo agregado|ct|Nou grup filla agregat",
            "Organizacion|ct|Organitzaci&oacute;",
            "Palabras clave|ct|?",
            "Participantes|ct|?",
            "Permite corregir errores|ct|Permet corregir errors",
            "Prevista de evaluacion|ct|?",
            "Prevista de evaluacion|ct|?",
            "Prevista de propuesta|ct|Prevista de proposta",
            "proceso.aquien.tip|ct|?",
            "proceso.aquien.titulo|ct|?",
            "proceso.conclusiones.tip|ct|?",
            "proceso.conclusiones.titulo|ct|?",
            "proceso.consecuancias.tip|ct|?",
            "proceso.consecuancias.titulo|ct|?",
            "proceso.definicion.tip|ct|?",
            "proceso.definicion.titulo|ct|?",
            "proceso.descripcion.tip|ct|?",
            "proceso.descripcion.titulo|ct|?",
            "proceso.entradas.tip|ct|?",
            "proceso.entradas.titulo|ct|?",
            "proceso.grupo.tip|ct|?",
            "proceso.grupo.titulo|ct|?",
            "proceso.implantacion.tip|ct|?",
            "proceso.implantacion.titulo|ct|?",
            "proceso.introduccion.tip|ct|Quina &eacute;s la situaci&oacute; actual?, Per qu&egrave; necessitem un nou procés operatiu?, Quins problemes resoldr&agrave;? Assegura't que la situaci&oacute; que descrius representi un problema real del grup",
            "proceso.introduccion.titulo|ct|?",
            "proceso.objetivo|ct|Quins s&oacute;n els objectius del nou grup de treball?",
            "proceso.objetivo.tip|ct|?",
            "proceso.objetivo.titulo|ct|?",
            "proceso.revision.tip|ct|?",
            "proceso.revision.titulo|ct|?",
            "proceso.valoracion.tip|ct|?",
            "proceso.valoracion.titulo|ct|?",
            "Proponer variante|ct|Proposar variant",
            "Recursos|ct|?",
            "Representante|ct|?",
            "Representantes|ct|?",
            "Revisar evaluacion|ct|?",
            "Revisar evaluacion|ct|?",
            "Revisar propuesta|ct|Revisar proposta",
            "Se deben definir los participantes|ct|?",
            "Seleccione un nodo|ct|Selecciona un node",
            "Simulacion|ct|Simulació!!!",
            "tema|ct|?",
            "tema|ct|?",
            "Titulo|ct|T&iacute;tol",
            "URL de la publicacion|ct|?",
            "Usuario [%1] actualizado|ct|Usu&agrave;ria [%1] actualitzada",
            "Usuario [%1] borrado|ct|Usu&agrave;ria [%1] borrada",
            "Usuario invalido|ct|Usuària inv&agrave;lida, operaci&oacute; registrada!",
            "Usuario no habilitado|ct|Usu&agrave;ria no habilitada per al grup [%1]",
            "Usuario o clave incorrectos|ct|Usu&agrave;ria o clau incorrectes per al grup [%1]",
            "Usuarios creados desahibilitados|ct|Usu&agrave;ries creades a [%1] deshabilitades, contacta la jardinera per a habilitar-les",
            "Vision|ct|?",

            "(Aparece en el pie del arbol)|en|(Appears at the foot of the tree)",
            "(Etiqueta en el arbol)|en|(Label in the tree)",
            "accion|en|Action",
            "Accion finalizada [%1]|en|action completed",
            "accion.aquien.tip|en|Who will benefit from the results",
            "accion.aquien.titulo|en|Who is it addressed to?",
            "accion.creada|en|Action created",
            "accion.descripcion.tip|en|Describe briefly how the action will be performed. Efforts, deadlines, costs, displacements, implications, phases.",
            "accion.descripcion.titulo|en|Description",
            "accion.error|en|Error",
            "accion.evaluacion.p1|en|Is this concrete action facilitating the development of our objectives? If not, why?",
            "accion.evaluacion.p2|en|Did we learn anything from the experience? If yes, what has it been? If not, what do you beleive has been the hindrance?",
            "accion.evaluacion.p3|en|Did you feel secured, included, participative during the process? If not, why?",
            "accion.evaluacion.p4|en|Phases, costs, ressources and timeline have been adequate? If not, why?",
            "accion.evaluacion.p5|en|Do you beleive the person leading the process has acted accordingly? If not, why?",
            "accion.evaluacion.p6|en|Do you beleive it is required a new cycle of action to attend the issue? If you think so, why?",
            "accion.evaluacion.tip1|en|Measure if the action was necessary as it was defined by the time the decision was taken",
            "accion.evaluacion.tip2|en|Measure if the defined objective of the action is in accordance with the obtained results",
            "accion.evaluacion.tip3|en|Measure you level of security, inclusiveness and participation during the executive phase",
            "accion.evaluacion.tip4|en|Measure in relation to phases, costs, ressources and timeline of the executed action",
            "accion.evaluacion.tip5|en|Measure in relation to the ability of the person leading the processes of the action",
            "accion.evaluacion.tip6|en|If so, explain the need of creating a new cycle of action to attend the issue resulting",
            "accion.fases.tip|en|Describe the steps to achieve the goal",
            "accion.fases.titulo|en|Phases",
            "accion.introduccion.tip|en|What is the situation that is requiring a proposal of action? Why Should we act? What could we improve? Please note that the situation you are describing represents the group",
            "accion.introduccion.titulo|en|Introduction",
            "accion.materiales.tip|en|Describe the resources that will be needed without forgetting an estimated budget",
            "accion.materiales.titulo|en|Materials",
            "accion.objetivo.tip|en|Describe what you want us to accomplish as a result of carrying out the proposed action. Motivate the group with this goal. Is it a group desire to achieve it?",
            "accion.objetivo.titulo|en|Goal to reach",
            "accion.presupuesto.tip|en|Estimation of costs and deadlines, the group must know and approve these values",
            "accion.presupuesto.titulo|en|Budget and deadline",
            "accion.responsable.tip|en|Project manager",
            "accion.responsable.titulo|en|Manager",
            "accion.revision.tip|en|Review",
            "accion.rrhh.tip|en|Describe the resources that will be needed without forgetting an estimated budget",
            "accion.rrhh.titulo|en|RRHH",
            "accion.software.tip|en|Describe the resources that will be needed without forgetting an estimated budget. Software, travel, services, etc.",
            "accion.software.titulo|en|Software",
            "Acta|en|Minute",
            "acta.apertura.tip|en|?",
            "acta.apertura.titulo|en|Opening round",
            "acta.evaluacion.tip|en|?",
            "acta.evaluacion.titulo|en|Evaluation",
            "acta.logisticos.tip|en|?",
            "acta.logisticos.titulo|en|Logistical aspects",
            "acta.ordendeldia.tip|en|?",
            "acta.ordendeldia.titulo|en|Agenda",
            "Agregar tema|en|Add theme",
            "alhijo.evaluacion.p1|en|The issue to attend has been well defined?",
            "alhijo.evaluacion.p2|en|The actual situation is in accordance to reality?",
            "alhijo.evaluacion.p3|en|The proposal requires analisis?",
            "alhijo.evaluacion.p4|en|The desired situation is assumable?",
            "alhijo.evaluacion.p5|en|This theme shall be adressed in this group?",
            "alhijo.evaluacion.tip1|en|?",
            "alhijo.evaluacion.tip2|en|?",
            "alhijo.evaluacion.tip3|en|?",
            "alhijo.evaluacion.tip4|en|?",
            "alhijo.evaluacion.tip5|en|?",
            "alhijo.introduccion.tip|en|?",
            "alhijo.introduccion.titulo|en|Introduction",
            "alhijo.propuesta.tip|en|?",
            "alhijo.propuesta.titulo|en|Proposal",
            "alhijo.situacionactual.tip|en|?",
            "alhijo.situacionactual.titulo|en|Current situation",
            "alhijo.situaciondeseada.tip|en|?",
            "alhijo.situaciondeseada.titulo|en|Desired situation",
            "alpadre.evaluacion.p1|en|The issue to attend has been well defined?",
            "alpadre.evaluacion.p2|en|The actual situation is in accordance to reality?",
            "alpadre.evaluacion.p3|en|The proposal requires analisis?",
            "alpadre.evaluacion.p4|en|The desired situation is assumable?",
            "alpadre.evaluacion.p5|en|This theme shall be adressed in this group?",
            "alpadre.evaluacion.tip1|en|?",
            "alpadre.evaluacion.tip2|en|?",
            "alpadre.evaluacion.tip3|en|?",
            "alpadre.evaluacion.tip4|en|?",
            "alpadre.evaluacion.tip5|en|?",
            "alpadre.introduccion.tip|en|?",
            "alpadre.introduccion.titulo|en|Introduction",
            "alpadre.propuesta.tip|en|?",
            "alpadre.propuesta.titulo|en|Proposal",
            "alpadre.situacionactual.tip|en|?",
            "alpadre.situacionactual.titulo|en|Current situation",
            "alpadre.situaciondeseada.tip|en|?",
            "alpadre.situaciondeseada.titulo|en|Desired situation",
            "Aspectos logisticos|en|Logistical aspects",
            "Autor de la publicacion|en|Author of the publication",
            "Cancelar|en|Cancel",
            "Cantidad de flores|en|Number of flowers must be between 1 and 20",
            "Capacidaddes|en|Capacities",
            "Cerrar|en|Close",
            "Comentarios|en|Comments",
            "Comunicado al grupo padre|en|Statement to the parent group",
            "Consecuencias|en|Consequenses",
            "Consenso alcanzado|en|Consent reached",
            "Crea la propuesta|en|Create proposal",
            "Crear evaluacion|en|Create evaluation",
            "Crear evaluacion|en|Create evaluation",
            "Crear nueva estrategia|en|Create new strategy",
            "Crear nuevo grupo de trabajo|en|Create new workgroup",
            "Crear propuesta|en|Create proposal",
            "Definir apertura|en|Define opening round",
            "Definir evaluacion|en|Define evaluation",
            "Definir orden del dia|en|Define agenda",
            "Definir un titulo de Acta|en|Define minute title",
            "Documento de comunicado a evaluar|en|Communication document to be evaluated",
            "Documento de resultado a evaluar|en|Result document to be evaluated",
            "Documento simulado|en|Simulated document",
            "El nodo no existe|en|Node does not exist",
            "El objetivo no puede ser vacio|en|Goal can not be empty",
            "El titulo del manifiesto no puede ser vacio|en|The title of the manifest can not be empty",
            "El usuario no existe|en|Username does not exist",
            "El usuario no existe o no esta habilitado|en|User does not exist or is disabled",
            "Eliminar estrategia|en|Delete strategy",
            "Eliminar grupo de trabajo|en|Delete workgroup",
            "enlace.evaluacion.p1|en|The theme is closely related to our aims? If so, in which manner?",
            "enlace.evaluacion.p2|en|Are we discussing or have discussed this theme in the group?",
            "enlace.evaluacion.p3|en|This theme should be integrated in our aims? If so, why?",
            "enlace.evaluacion.p4|en|Do you think this theme could force us to review our aims?",
            "enlace.evaluacion.p5|en|Do you think this theme should generate a new cycle of actions? If so, why?",
            "enlace.evaluacion.tip1|en|Measure if this theme is aligned with the interests of the group? Could it bring some valuable information?",
            "enlace.evaluacion.tip2|en|Measure if this theme is deeply relevant and impactful?",
            "enlace.evaluacion.tip3|en|Measure if this theme can facilitate the development of our aims?",
            "enlace.evaluacion.tip4|en|Do you consider that this theme could modify our aims?",
            "enlace.evaluacion.tip5|en|Do you consider that this theme could provoke real actions in our group?",
            "Enseña vista previa antes de proponer|en|Show preview before proposing",
            "Enviar|en|Send",
            "Este debate ya ha alcanzado el consenso|en|This discussion has already reached consent",
            "Este manifiesto reemplaza al anterior|en|This manifest replaces the previous one",
            "estrategia.actualizada|en|Updated strategy in the workgroup [%1] in the organizational structure",
            "estrategia.creada|en|Strategy created in the workgroup [%1] in the organizational structure",
            "estrategia.eliminada|en|Strategy removed from workgroup [%1] in organizational structure",
            "Estructura organizativa actualizada|en|Organizational structure updated",
            "Etiqueta|en|Label",
            "Evaluacion caida para el tema [%1]|en|Evaluation has droped for this theme",
            "Evaluacion de Accion|en|Evaluation of action",
            "Evaluación de Accion|en|Evaluation of action",
            "Evaluacion de comunicado intergrupal|en|Intergroup communication evaluation",
            "Evaluacion de Enlace|en|Evaluation of Link",
            "Evaluacion de Evento|en|Evaluation of Event",
            "evento.aquien.tip|en|To whom is the event directed to, and who shall benefit from the results",
            "evento.aquien.titulo|en|To whom is it adressed",
            "evento.creado|en|Event created",
            "evento.descripcion.tip|en|Describe how will the event be Show? Presentation? Speaker? Music? Activities? Cathering?",
            "evento.descripcion.titulo|en|Description",
            "evento.error|en|Error",
            "evento.evaluacion.p1|en|Is this event facilitating the development of our objectives? If not, why?",
            "evento.evaluacion.p2|en|Did we learn anything from the experience? If yes, what has it been? If not, what do you beleive has been the hindrance?",
            "evento.evaluacion.p3|en|Did you feel secured, included, participative during the process? If not, why?",
            "evento.evaluacion.p4|en|Phases, costs, ressources and timeline have been adequate? If not, why?",
            "evento.evaluacion.p5|en|Do you believe the person leading the process has acted accordingly? If not, why?",
            "evento.evaluacion.p6|en|Do you believe it is required a new cycle of action to attend the issue? If you think so, why?",
            "evento.evaluacion.tip1|en|Measure if the action was necessary as it was defined by the time the decision was taken",
            "evento.evaluacion.tip2|en|Measure if the defined objective of the action is in accordance with the obtained results",
            "evento.evaluacion.tip3|en|Measure you level of security, inclusiveness and participation during the executive phase",
            "evento.evaluacion.tip4|en|Measure in relation to phases, costs, ressources and timeline of the executed action",
            "evento.evaluacion.tip5|en|Measure in relation to the ability of the person leading the processes of the action",
            "evento.evaluacion.tip6|en|If so, explain the need of create a new cycle of action to attend the issue resulting",
            "evento.fecha.tip|en|Propose a date for the event",
            "evento.fecha.titulo|en|Propose a date for the event",
            "evento.introduccion.tip|en|What is the situation that is requiring a proposal of event? Why Should we act? What could we improve? Please note that the situation you are describing represents the group",
            "evento.introduccion.titulo|en|Introduction",
            "evento.lugar.tip|en|Describe the ressources needed without forgetting estimated quotation",
            "evento.lugar.titulo|en|Place",
            "evento.materiales.tip|en|Describe the ressources needed without forgetting estimated quotation",
            "evento.materiales.titulo|en|Materials",
            "evento.objetivo.tip|en|Descibe what do you pretend we achieve by realizing this event. Motivate the team with this aim. Is it a group desire to achieve it?",
            "evento.objetivo.titulo|en|Aim to achieve",
            "evento.organizacion.tip|en|Describe how we should organize this event, step by step, timing, displacements, hiring, etc.",
            "evento.organizacion.titulo|en|Organization",
            "evento.responsable.tip|en|?",
            "evento.responsable.titulo|en|Supervisor",
            "evento.transportes.tip|en|Describe ressources needed without forgetting estimated quotation. Software, displacements, Services, etc.",
            "evento.transportes.titulo|en|Transportation",
            "evento.valoracion.tip|en|How shall we measure the results of this event? Sold tickets?, Amount of people assisting?, Achieved recognition?, Social effects?, Advertizement? The measurement of the event shall be quantifiable and easy the understand. It should be clear whether or not result is acceptable.",
            "evento.valoracion.titulo|en|Result assessment",
            "Fecha|en|Date",
            "Fecha|en|Date",
            "Fecha celebracion|en|Date celebration",
            "Fecha celebracion|en|Date celebration",
            "Fecha proxima celebracion|en|Date next celebration",
            "Fecha proxima celebracion|en|Date next celebration",
            "Grupo hijo borrado|en|Child group deleted ",
            "grupoTrabajo.actualizado.tip|en|Workgroup actualized in organizational structure",
            "grupoTrabajo.actualizado.titulo|en|Workgroup actualized in organizational structure",
            "grupoTrabajo.aquien.tip|en|Who is benefited from the results",
            "grupoTrabajo.aquien.titulo|en|To whom it is addressed",
            "grupoTrabajo.capacidades.tip|en|What kind of abilities shall have people composing the new workgroup? Knowledge, experiences, etc.",
            "grupoTrabajo.capacidades.titulo|en|Needed abilities",
            "grupoTrabajo.conclusiones.tip|en|What did we learn through this experience?",
            "grupoTrabajo.conclusiones.titulo|en|Conclusions",
            "grupoTrabajo.consecuencias.tip|en|What are the consequences of eliminating this workgroup we would have to resolve?",
            "grupoTrabajo.consecuencias.titulo|en|Consequences",
            "grupoTrabajo.creado.tip|en|Workgroup created in the organizational structure",
            "grupoTrabajo.creado.titulo|en|Workgroup created in the organizational structure",
            "grupoTrabajo.descripcion.tip|en|Describe briefly activities to realize to reach our aims",
            "grupoTrabajo.descripcion.titulo|en|Description",
            "grupoTrabajo.eliminado.tip|en|Workgroup removed from the organizational structure",
            "grupoTrabajo.eliminado.titulo|en|Workgroup removed from the organizational structure",
            "grupoTrabajo.integrantes.tip|en|Who will be the members of the new work group? These names serve as a proposal or reference. They do not really involve people. If you do not want to imply real names, say 'to define'",
            "grupoTrabajo.integrantes.titulo|en|Members",
            "grupoTrabajo.introduccion.tip|en|What is the current situation? Why do we need a new working group? What problems will it solve? Keep in mind that the situation you describe represents the group",
            "grupoTrabajo.introduccion.titulo|en|Introduction",
            "grupoTrabajo.objetivo.tip|en|What are the aims of the new workgroup?",
            "grupoTrabajo.objetivo.titulo|en|Goal to achieve",
            "grupoTrabajo.recursos.tip|en|What resources would be necessary for the new working group? Furniture, computers, work place, etc.?",
            "grupoTrabajo.recursos.titulo|en|Resources",
            "grupoTrabajo.revision.tip|en|What would be the periodicity to evaluate the results achieved?",
            "grupoTrabajo.revision.titulo|en|Assessment review of the results",
            "grupoTrabajo.valoracion.tip|en|How will be measured the result of this working group? Number of processes executed? Efficiency metrics? Recognition of other people? Concrete results?. <br> The assessment of group performance should be quantifiable and easy to understand. It must be clear to everyone if it meets its objective or not.",
            "grupoTrabajo.valoracion.titulo|en|Result assessment",
            "Integrantes|en|Members",
            "La clave actual no corresponde|en|Key does not match",
            "La clave nueva no puede ser vacia|en|New key can not be empty",
            "La evaluacion anterior ha sido reemplazada|en|Last evaluation has been replaced",
            "La mision no ser vacia|en|Mission can not be empty",
            "La vision no puede ser vacia|en|Vision can not be empty",
            "Lugar|en|Place",
            "Manifiesto|en|Manifest",
            "Manifiesto actualizado en el grupo|en|Manifest updated in the group",
            "manifiesto.mision.tip|en|The mission defines what our work is, what we do, the relevant social sector, who are our target audience, what is our geographic scope of action.",
            "manifiesto.mision.tip|en|Mission",
            "manifiesto.mision.tip|en|The mission defines what our work is, what we do, the relevant social sector, who are our target audience, what is our geographic scope of action.",
            "manifiesto.mision.tip|en|Mission",
            "manifiesto.objetivos.tip|en|Aims shall help us to concrete the mission. They give a sense of time and define a field of service. They shall show the mutual benefit between the organization and its environment",
            "manifiesto.objetivos.titulo|en|Aims",
            "manifiesto.servicios.tip|en|What services do we provide to achieve our aims? These services are the starting point for the creation of operational processes and working groups. List named services and a brief description",
            "manifiesto.servicios.titulo|en|Services",
            "manifiesto.vision.tip|en|The vision has an inspiring and motivating character for the group. What vision do we have of the world? How should things be?",
            "max|en|Max",
            "Maximos usuarios negados|en|Maximum denied users must be between 0 and 50",
            "Minimos usuarios implicados|en|Minimal users involved should be between 50 and 100",
            "Modificar estrategia|en|Modify strategy",
            "Modificar grupo de trabajo|en|Modify workgroup",
            "Nivel [%1] no existe en este modelo|en|Level [%1] does not exist on this form",
            "Nivel en el arbol|en|Tree level",
            "No hay grupos hijos en este grupo|en|There is no sons group on this group",
            "No tiene flores para crear una propuesta|en|No flowers to create a proposal",
            "Nombre|en|Name",
            "Nombre de la publicacion|en|Name of the publication",
            "Nombre de nodo no puede ser vacio|en|Name of node can not be empty",
            "Nueva Acta publicada|en|New minute publised",
            "Nueva decision [%1]|en|New decision",
            "Nuevo grupo hijo agregado|en|New child group added",
            "Organizacion|en|Organization",
            "Palabras clave|en|Key words",
            "Participantes|en|Participants",
            "Permite corregir errores|en|Allows to correct errors",
            "Prevista de evaluacion|en|Evaluation preview",
            "Prevista de evaluacion|en|Evaluation preview",
            "Prevista de propuesta|en|Proposal preview",
            "proceso.aquien.tip|en|Who will benefit from the results of using this strategy",
            "proceso.aquien.titulo|en|To whom it is addressed",
            "proceso.conclusiones.tip|en|What did we learn from this experience?",
            "proceso.conclusiones.titulo|en|Conclusions",
            "proceso.consecuancias.tip|en|What are the consequences of eliminating this working group?",
            "proceso.consecuancias.titulo|en|What are the consequences of eliminating this working group?",
            "proceso.definicion.tip|en|Define step by step the tasks to be performed when this strategy is executed. Define how the exceptions will be treated. A strategy is a flow of conditions and tasks that includes all possible alternatives so that its execution does not admit discussions about its results. A well-defined strategy is an important part of the operational definition of the group. All the usual activities of the group should be defined as strategies.",
            "proceso.definicion.titulo|en|Strategy definition",
            "proceso.descripcion.tip|en|Briefly describe the steps to take to use this strategy. Mention how exceptional cases will be treated. A strategy must always generate a result, even if it is a failure record for later analysis.",
            "proceso.descripcion.titulo|en|Description",
            "proceso.entradas.tip|en|How is the execution of this process triggered? What do you need for this process to be able to run? A new partner, a document, a personal request, etc.",
            "proceso.entradas.titulo|en|Strategy entry",
            "proceso.grupo.tip|en|Which workgroup uses this strategy?",
            "proceso.grupo.titulo|en|Workgroup",
            "proceso.implantacion.tip|en|What should be done to implement this strategy? Resources, physical site, knowledge and capacities of people, forms / data sheets / information brochures, software, etc.",
            "proceso.implantacion.titulo|en|Implantation",
            "proceso.introduccion.tip|en|What is the current situation? Do we need a new operational process? What problems will it solve? Note that the situation you describe represents a real problem for the group",
            "proceso.introduccion.titulo|en|What is the current situation? Why do we need a new strategy? What problems will it solve? Keep in mind that the situation you describe represents a real problem for the group",
            "proceso.objetivo|en|What are the objectives of the new working group?",
            "proceso.objetivo.tip|en|?",
            "proceso.objetivo.titulo|en|Aim",
            "proceso.revision.tip|en|When will be reviewed the definition of this strategy?",
            "proceso.revision.titulo|en|Assessment review of result",
            "proceso.valoracion.tip|en|How will be measured the result of this strategy? Number of processes executed? results obtained? Is it need resolving? Is it easy to execute?. <br> The evaluation of an operational process must be quantifiable and easy to understand. It must be clear to everyone if it meets its objective or not.?",
            "proceso.valoracion.titulo|en|Result assessment",
            "Proponer variante|en|Suggest a variant",
            "Recursos|en|Ressources",
            "Representante|en|Representative",
            "Representantes|en|Representatives",
            "Revisar evaluacion|en|Evaluation review",
            "Revisar evaluacion|en|Evaluation review",
            "Revisar propuesta|en|Check proposal",
            "Se deben definir los participantes|en|The participants must be defined",
            "Seleccione un nodo|en|Select a tree node",
            "Simulacion|en|Simulation!!!",
            "tema|en|Theme",
            "tema|en|Theme",
            "Titulo|en|Title",
            "URL de la publicacion|en|URL of publication",
            "Usuario [%1] actualizado|en|User [%1] updated",
            "Usuario [%1] borrado|en|User [%1] deleted",
            "Usuario invalido|en|Ivalid user, operation registered!",
            "Usuario no habilitado|en|User disabled for group [%1]",
            "Usuario o clave incorrectos|en|User or password incorrect for the group [%1]",
            "Usuarios creados desahibilitados|en|Disabled users created in [%1], contact the Operational Leader to enable them",
            "Vision|en|Vision",

            "(Aparece en el pie del arbol)|fr|?",
            "(Etiqueta en el arbol)|fr|?",
            "accion|fr|?",
            "Accion finalizada [%1]|fr|?",
            "accion.aquien.tip|fr|?",
            "accion.aquien.titulo|fr|?",
            "accion.creada|fr|?",
            "accion.descripcion.tip|fr|?",
            "accion.descripcion.titulo|fr|?",
            "accion.error|fr|?",
            "accion.evaluacion.p1|fr|?",
            "accion.evaluacion.p2|fr|?",
            "accion.evaluacion.p3|fr|?",
            "accion.evaluacion.p4|fr|?",
            "accion.evaluacion.p5|fr|?",
            "accion.evaluacion.p6|fr|?",
            "accion.evaluacion.tip1|fr|?",
            "accion.evaluacion.tip2|fr|?",
            "accion.evaluacion.tip3|fr|?",
            "accion.evaluacion.tip4|fr|?",
            "accion.evaluacion.tip5|fr|?",
            "accion.evaluacion.tip6|fr|?",
            "accion.fases.tip|fr|?",
            "accion.fases.titulo|fr|?",
            "accion.introduccion.tip|fr|?",
            "accion.introduccion.titulo|fr|?",
            "accion.materiales.tip|fr|?",
            "accion.materiales.titulo|fr|?",
            "accion.objetivo.tip|fr|?",
            "accion.objetivo.titulo|fr|?",
            "accion.presupuesto.tip|fr|?",
            "accion.presupuesto.titulo|fr|?",
            "accion.responsable.tip|fr|?",
            "accion.responsable.titulo|fr|?",
            "accion.revision.tip|fr|?",
            "accion.rrhh.tip|fr|?",
            "accion.rrhh.titulo|fr|?",
            "accion.software.tip|fr|?",
            "accion.software.titulo|fr|?",
            "Acta|fr|?",
            "acta.apertura.tip|fr|?",
            "acta.apertura.titulo|fr|?",
            "acta.evaluacion.tip|fr|?",
            "acta.evaluacion.titulo|fr|?",
            "acta.logisticos.tip|fr|?",
            "acta.logisticos.titulo|fr|?",
            "acta.ordendeldia.tip|fr|?",
            "acta.ordendeldia.titulo|fr|?",
            "Agregar tema|fr|?",
            "alhijo.evaluacion.p1|fr|?",
            "alhijo.evaluacion.p2|fr|?",
            "alhijo.evaluacion.p3|fr|?",
            "alhijo.evaluacion.p4|fr|?",
            "alhijo.evaluacion.p5|fr|?",
            "alhijo.evaluacion.tip1|fr|?",
            "alhijo.evaluacion.tip2|fr|?",
            "alhijo.evaluacion.tip3|fr|?",
            "alhijo.evaluacion.tip4|fr|?",
            "alhijo.evaluacion.tip5|fr|?",
            "alhijo.introduccion.tip|fr|?",
            "alhijo.introduccion.titulo|fr|?",
            "alhijo.propuesta.tip|fr|?",
            "alhijo.propuesta.titulo|fr|?",
            "alhijo.situacionactual.tip|fr|?",
            "alhijo.situacionactual.titulo|fr|?",
            "alhijo.situaciondeseada.tip|fr|?",
            "alhijo.situaciondeseada.titulo|fr|?",
            "alpadre.evaluacion.p1|fr|?",
            "alpadre.evaluacion.p2|fr|?",
            "alpadre.evaluacion.p3|fr|?",
            "alpadre.evaluacion.p4|fr|?",
            "alpadre.evaluacion.p5|fr|?",
            "alpadre.evaluacion.tip1|fr|?",
            "alpadre.evaluacion.tip2|fr|?",
            "alpadre.evaluacion.tip3|fr|?",
            "alpadre.evaluacion.tip4|fr|?",
            "alpadre.evaluacion.tip5|fr|?",
            "alpadre.introduccion.tip|fr|?",
            "alpadre.introduccion.titulo|fr|?",
            "alpadre.propuesta.tip|fr|?",
            "alpadre.propuesta.titulo|fr|?",
            "alpadre.situacionactual.tip|fr|?",
            "alpadre.situacionactual.titulo|fr|?",
            "alpadre.situaciondeseada.tip|fr|?",
            "alpadre.situaciondeseada.tip|fr|?",
            "Aspectos logisticos|fr|?",
            "Autor de la publicacion|fr|?",
            "Cancelar|fr|?",
            "Cantidad de flores|fr|?",
            "Capacidaddes|fr|?",
            "Cerrar|fr|?",
            "Comentarios|fr|?",
            "Comunicado al grupo padre|fr|?",
            "Consecuencias|fr|?",
            "Consenso alcanzado|fr|?",
            "Crea la propuesta|fr|?",
            "Crear evaluacion|fr|?",
            "Crear evaluacion|fr|?",
            "Crear nueva estrategia|fr|?",
            "Crear nuevo grupo de trabajo|fr|?",
            "Crear propuesta|fr|?",
            "Definir apertura|fr|?",
            "Definir evaluacion|fr|?",
            "Definir orden del dia|fr|?",
            "Definir un titulo de Acta|fr|?",
            "Documento de comunicado a evaluar|fr|?",
            "Documento de resultado a evaluar|fr|?",
            "Documento simulado|fr|?",
            "El nodo no existe|fr|?",
            "El objetivo no puede ser vacio|fr|?",
            "El titulo del manifiesto no puede ser vacio|fr|?",
            "El usuario no existe|fr|?",
            "El usuario no existe o no esta habilitado|fr|?",
            "Eliminar estrategia|fr|?",
            "Eliminar grupo de trabajo|fr|?",
            "enlace.evaluacion.p1|fr|?",
            "enlace.evaluacion.p2|fr|?",
            "enlace.evaluacion.p3|fr|?",
            "enlace.evaluacion.p4|fr|?",
            "enlace.evaluacion.p5|fr|?",
            "enlace.evaluacion.tip1|fr|?",
            "enlace.evaluacion.tip2|fr|?",
            "enlace.evaluacion.tip3|fr|?",
            "enlace.evaluacion.tip4|fr|?",
            "enlace.evaluacion.tip5|fr|?",
            "Enseña vista previa antes de proponer|fr|?",
            "Enviar|fr|?",
            "Este debate ya ha alcanzado el consenso|fr|?",
            "Este manifiesto reemplaza al anterior|fr|?",
            "estrategia.actualizada|fr|?",
            "estrategia.creada|fr|?",
            "estrategia.eliminada|fr|?",
            "Estructura organizativa actualizada|fr|?",
            "Etiqueta|fr|?",
            "Evaluacion caida para el tema [%1]|fr|?",
            "Evaluacion de Accion|fr|?",
            "Evaluación de Accion|fr|?",
            "Evaluacion de comunicado intergrupal|fr|?",
            "Evaluacion de Enlace|fr|?",
            "Evaluacion de Evento|fr|?",
            "evento.aquien.tip|fr|?",
            "evento.aquien.titulo|fr|?",
            "evento.creado|fr|?",
            "evento.descripcion.tip|fr|?",
            "evento.descripcion.titulo|fr|?",
            "evento.error|fr|?",
            "evento.evaluacion.p1|fr|?",
            "evento.evaluacion.p2|fr|?",
            "evento.evaluacion.p3|fr|?",
            "evento.evaluacion.p4|fr|?",
            "evento.evaluacion.p5|fr|?",
            "evento.evaluacion.p6|fr|?",
            "evento.evaluacion.tip1|fr|?",
            "evento.evaluacion.tip2|fr|?",
            "evento.evaluacion.tip3|fr|?",
            "evento.evaluacion.tip4|fr|?",
            "evento.evaluacion.tip5|fr|?",
            "evento.evaluacion.tip6|fr|?",
            "evento.fecha.tip|fr|?",
            "evento.fecha.titulo|fr|?",
            "evento.introduccion.tip|fr|?",
            "evento.introduccion.titulo|fr|?",
            "evento.lugar.tip|fr|?",
            "evento.lugar.titulo|fr|?",
            "evento.materiales.tip|fr|?",
            "evento.materiales.titulo|fr|?",
            "evento.objetivo.tip|fr|?",
            "evento.objetivo.titulo|fr|?",
            "evento.organizacion.tip|fr|?",
            "evento.organizacion.titulo|fr|?",
            "evento.responsable.tip|fr|?",
            "evento.responsable.titulo|fr|?",
            "evento.transportes.tip|fr|?",
            "evento.transportes.titulo|fr|?",
            "evento.valoracion.tip|fr|?",
            "evento.valoracion.titulo|fr|?",
            "Fecha|fr|?",
            "Fecha|fr|?",
            "Fecha celebracion|fr|?",
            "Fecha celebracion|fr|?",
            "Fecha proxima celebracion|fr|?",
            "Fecha proxima celebracion|fr|?",
            "Grupo hijo borrado|fr|?",
            "grupoTrabajo.actualizado.tip|fr|?",
            "grupoTrabajo.actualizado.titulo|fr|?",
            "grupoTrabajo.aquien.tip|fr|?",
            "grupoTrabajo.aquien.titulo|fr|?",
            "grupoTrabajo.capacidades.tip|fr|?",
            "grupoTrabajo.capacidades.titulo|fr|?",
            "grupoTrabajo.conclusiones.tip|fr|?",
            "grupoTrabajo.conclusiones.titulo|fr|?",
            "grupoTrabajo.consecuencias.tip|fr|?",
            "grupoTrabajo.consecuencias.titulo|fr|?",
            "grupoTrabajo.creado.tip|fr|?",
            "grupoTrabajo.creado.titulo|fr|?",
            "grupoTrabajo.descripcion.tip|fr|?",
            "grupoTrabajo.descripcion.titulo|fr|?",
            "grupoTrabajo.eliminado.tip|fr|?",
            "grupoTrabajo.eliminado.titulo|fr|?",
            "grupoTrabajo.integrantes.tip|fr|?",
            "grupoTrabajo.integrantes.titulo|fr|?",
            "grupoTrabajo.introduccion.tip|fr|?",
            "grupoTrabajo.introduccion.titulo|fr|?",
            "grupoTrabajo.objetivo.tip|fr|?",
            "grupoTrabajo.objetivo.titulo|fr|?",
            "grupoTrabajo.recursos.tip|fr|?",
            "grupoTrabajo.recursos.titulo|fr|?",
            "grupoTrabajo.revision.tip|fr|?",
            "grupoTrabajo.revision.titulo|fr|?",
            "grupoTrabajo.valoracion.tip|fr|?",
            "grupoTrabajo.valoracion.titulo|fr|?",
            "Integrantes|fr|?",
            "La clave actual no corresponde|fr|?",
            "La clave nueva no puede ser vacia|fr|?",
            "La evaluacion anterior ha sido reemplazada|fr|?",
            "La mision no ser vacia|fr|?",
            "La vision no puede ser vacia|fr|?",
            "Lugar|fr|?",
            "Manifiesto|fr|?",
            "Manifiesto actualizado en el grupo|fr|?",
            "manifiesto.mision.tip|fr|?",
            "manifiesto.mision.tip|fr|?",
            "manifiesto.objetivos.tip|fr|?",
            "manifiesto.objetivos.titulo|fr|?",
            "manifiesto.servicios.tip|fr|?",
            "manifiesto.servicios.titulo|fr|?",
            "manifiesto.vision.tip|fr|?",
            "max|fr|?",
            "Maximos usuarios negados|fr|?",
            "Minimos usuarios implicados|fr|?",
            "Modificar estrategia|fr|?",
            "Modificar grupo de trabajo|fr|?",
            "Nivel [%1] no existe en este modelo|fr|?",
            "Nivel en el arbol|fr|?",
            "No hay grupos hijos en este grupo|fr|?",
            "No tiene flores para crear una propuesta|fr|?",
            "Nombre|fr|?",
            "Nombre de la publicacion|fr|?",
            "Nombre de nodo no puede ser vacio|fr|?",
            "Nueva Acta publicada|fr|?",
            "Nueva decision [%1]|fr|?",
            "Nuevo grupo hijo agregado|fr|?",
            "Organizacion|fr|?",
            "Palabras clave|fr|?",
            "Participantes|fr|?",
            "Permite corregir errores|fr|?",
            "Prevista de evaluacion|fr|?",
            "Prevista de evaluacion|fr|?",
            "Prevista de propuesta|fr|?",
            "proceso.aquien.tip|fr|?",
            "proceso.aquien.titulo|fr|?",
            "proceso.conclusiones.tip|fr|?",
            "proceso.conclusiones.titulo|fr|?",
            "proceso.consecuancias.tip|fr|?",
            "proceso.consecuancias.titulo|fr|?",
            "proceso.definicion.tip|fr|?",
            "proceso.definicion.titulo|fr|?",
            "proceso.descripcion.tip|fr|?",
            "proceso.descripcion.titulo|fr|?",
            "proceso.entradas.tip|fr|?",
            "proceso.entradas.titulo|fr|?",
            "proceso.grupo.tip|fr|?",
            "proceso.grupo.titulo|fr|?",
            "proceso.implantacion.tip|fr|?",
            "proceso.implantacion.titulo|fr|?",
            "proceso.introduccion.tip|fr|?",
            "proceso.introduccion.titulo|fr|?",
            "proceso.objetivo|fr|?",
            "proceso.objetivo.tip|fr|?",
            "proceso.objetivo.titulo|fr|?",
            "proceso.revision.tip|fr|?",
            "proceso.revision.titulo|fr|?",
            "proceso.valoracion.tip|fr|?",
            "proceso.valoracion.titulo|fr|?",
            "Proponer variante|fr|?",
            "Recursos|fr|?",
            "Representante|fr|?",
            "Representantes|fr|?",
            "Revisar evaluacion|fr|?",
            "Revisar evaluacion|fr|?",
            "Revisar propuesta|fr|?",
            "Se deben definir los participantes|fr|?",
            "Seleccione un nodo|fr|?",
            "Simulacion|fr|?",
            "tema|fr|?",
            "tema|fr|?",
            "Titulo|fr|?",
            "URL de la publicacion|fr|?",
            "Usuario [%1] actualizado|fr|?",
            "Usuario [%1] borrado|fr|?",
            "Usuario invalido|fr|?",
            "Usuario no habilitado|fr|?",
            "Usuario o clave incorrectos|fr|?",
            "Usuarios creados desahibilitados|fr|?",
            "Vision|fr|?"
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
                    if (ret != "" && ret!="?")
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

        public static string sendMailAlta(Grupo grupo, string to, string nombre, string email, string basepath)
        {
            string msg = System.IO.File.ReadAllText(basepath + "\\" + grupo.idioma + "\\alta.html");
            msg = msg.Replace("%1", nombre);
            msg = msg.Replace("%2", email);
            msg = msg.Replace("%3", grupo.nombre);
            msg = msg.Replace("%4", "<a href='" + grupo.URL + "/default.html?grupo=" + grupo.nombre + "' target='_blank'>" + grupo.URL + "/default.html?grupo=" + grupo.nombre + "</a>");

            return sendMail(to, "Solicitud de alta en [" + grupo.nombre + "]", msg);
        }

        public static string encolarMailCaido(Grupo grupo, string to, string mailAdmin, string basepath)
        {
            string msg = System.IO.File.ReadAllText(basepath + "\\caido.html");
            msg = msg.Replace("%1", grupo.nombre);
            msg = msg.Replace("%2", "<a href='" + grupo.URL + "/default.html?grupo=" + grupo.nombre + "' target='_blank'>" + grupo.URL + "/default.html?grupo=" + grupo.nombre + "</a>");

            return encolarMail(to, "Flores caducadas", msg);
        }

        public static string sendMailWelcome(Grupo grupo, string to, string clave, string url, string basepath)
        {
            string msg = System.IO.File.ReadAllText(basepath + "\\welcome.html");
            msg = msg.Replace("%1", url);
            msg = msg.Replace("%2", to);
            msg = msg.Replace("%3", clave);
            msg = msg.Replace("%4", grupo.nombre);
            msg = msg.Replace("%5", "<a href='" + grupo.URL + "/default.html?grupo=" + grupo.nombre + "' target='_blank'>" + grupo.URL + "/default.html?grupo=" + grupo.nombre + "</a>");

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
                    msg.IsBodyHtml = true;
                    msg.Body = body.Replace("\"", "'").Replace("\r", "").Replace("\n", "");
                    msg.Subject = subject;
                    msg.To.Add(new System.Net.Mail.MailAddress(to, to));

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(SMTPURL);
                    smtp.EnableSsl = false;                                                    //esto en la CIC no funciona
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

        public static string encolarMailInactivo(Grupo grupo, string to)
        {
            string msg = System.IO.File.ReadAllText(startupPath + "\\mails\\modelos\\ES\\inactivo.html");
            msg = msg.Replace("%1", "<a href='" + grupo.URL + "/default.html?grupo=" + grupo.nombre + "' target='_blank'>" + grupo.URL + "/default.html?grupo=" + grupo.nombre + "</a>");

            return encolarMail(to, "Inactivo en Nabu", msg);
        }

        public static string encolarMailNuevoConsenso(Grupo grupo, string to, int si, int no, string link)
        {
            string msg = System.IO.File.ReadAllText(startupPath + "\\mails\\modelos\\ES\\nuevoConsenso.html");
            msg = msg.Replace("%1", si.ToString());
            msg = msg.Replace("%2", no.ToString());
            msg = msg.Replace("%3", link);
            msg = msg.Replace("%4", link);
            msg = msg.Replace("%5", "<a href='" + grupo.URL + "/default.html?grupo=" + grupo.nombre + "' target='_blank'>" + grupo.URL + "/default.html?grupo=" + grupo.nombre + "</a>");

            return encolarMail(to, "Nueva decisión", msg);
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
