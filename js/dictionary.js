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

var idioma = "";

var dictionary = [
    { key: 'Crear nuevo grupo', idioma: 'es', txt: 'Crear nuevo grupo' },
    { key: 'Grupos', idioma: 'es', txt: 'Grupos' },
    { key: 'auTit2', idioma: 'es', txt: '<br /><br />El alta de usuarios es administrada manualmente.<br /><br />Selecciona el grupo<br /> y completa tus datos.<br />Recibiras un email con tus datos de acceso.<br /><br />' },
    { key: 'ufTit1', idioma: 'es', txt: 'Si quitas la &uacute;ltima flor la propuesta dejar&aacute; de existir' },
    { key: 'Se perderan los datos no guardados', idioma: 'es', txt: 'Se perder&aacute;n los datos no guardados' },
    { key: 'Nombre no puede ser vacio', idioma: 'es', txt: 'Nombre no puede ser vac&iacute;o' },
    { key: 'Email no puede ser vacio', idioma: 'es', txt: 'Email no puede ser vac&iacute;o' },
    { key: 'Mensaje enviado al coordinador', idioma: 'es', txt: 'Mensaje enviado al coordinador' },
    { key: 'Las nuevas claves no coinciden', idioma: 'es', txt: 'Las nuevas claves no coinciden' },
    { key: 'Solo disponible para el coordinador', idioma: 'es', txt: 'Solo disponible para el coordinador' },
    { key: 'Apoyos', idioma: 'es', txt: 'Apoyos' },
    { key: 'Ultimo', idioma: 'es', txt: 'Ultimo' },
    { key: 'Activo', idioma: 'es', txt: 'Activo' },
    { key: 'Solo Lectura', idioma: 'es', txt: 'Solo lectura' },
    { key: 'Habilitado', idioma: 'es', txt: 'Habilitado' },
    { key: 'Secretaria', idioma: 'es', txt: 'Secretaria' },
    { key: 'abrSoloLectura', idioma: 'es', txt: 'Solo Lect' },
    { key: 'abrHabilitado', idioma: 'es', txt: 'Habilitado' },
    { key: 'abrCoordinador', idioma: 'es', txt: 'Coordinador' },
    { key: 'abrSecretaria', idioma: 'es', txt: 'Secretaria' },
    { key: 'abrRepresentante', idioma: 'es', txt: 'Representante' },
    { key: 'abrFacilitador', idioma: 'es', txt: 'Facilitador' },
    { key: 'Email', idioma: 'es', txt: 'Email' },
    { key: 'Borrar', idioma: 'es', txt: 'Borrar' },
    { key: 'No se puede borrar el usuario actual', idioma: 'es', txt: 'No se puede borrar el usuario actual' },
    { key: 'Estructuras organizativas', idioma: 'es', txt: 'Estructuras organizativas' },
    { key: 'Cerrar', idioma: 'es', txt: 'Cerrar' },
    { key: 'Grupo padre no corresponde', idioma: 'es', txt: 'Grupo padre no corresponde' },
    { key: 'Usuarios', idioma: 'es', txt: 'Usuarios' },
    { key: 'Decisiones', idioma: 'es', txt: 'Decisiones' },
    { key: 'Resultados', idioma: 'es', txt: 'Resultados' },
    { key: 'Lunes', idioma: 'es', txt: 'Lunes' },
    { key: 'ene', idioma: 'es', txt: 'ene' },
    { key: 'feb', idioma: 'es', txt: 'feb' },
    { key: 'mar', idioma: 'es', txt: 'mar' },
    { key: 'abr', idioma: 'es', txt: 'abr' },
    { key: 'may', idioma: 'es', txt: 'may' },
    { key: 'jun', idioma: 'es', txt: 'jun' },
    { key: 'jul', idioma: 'es', txt: 'jul' },
    { key: 'ago', idioma: 'es', txt: 'ago' },
    { key: 'set', idioma: 'es', txt: 'set' },
    { key: 'oct', idioma: 'es', txt: 'oct' },
    { key: 'nov', idioma: 'es', txt: 'nov' },
    { key: 'dic', idioma: 'es', txt: 'dic' },
    { key: 'URL', idioma: 'es', txt: 'URL' },
    { key: 'conminSiPc', idioma: 'es', txt: 'Minimos usuarios implicados en el debate. Cada rama que parte de la raiz es un debate distinto.' },
    { key: 'conmaxNoPc', idioma: 'es', txt: 'M&aacute;ximos usuarios que divergen en el debate. Si hay mas entonces no se alcanza el consenso.' },
    { key: 'Pausa', idioma: 'es', txt: 'Pausa' },
    { key: 'Diverger', idioma: 'es', txt: 'Divergir' },
    { key: 'Converger', idioma: 'es', txt: 'Converger' },
    { key: 'Sociocracia', idioma: 'es', txt: 'Sociocracia<br>Digital' },
    { key: 'Democracia interactiva', idioma: 'es', txt: 'Democracia<br>interactiva' },
    { key: 'El bosque', idioma: 'es', txt: 'Comunidad' },
    { key: 'El arbol', idioma: 'es', txt: 'Grupo' },
    { key: 'Mailer', idioma: 'es', txt: 'Mailer' },
    { key: 'Dias', idioma: 'es', txt: 'D&iacute;as' },
    { key: 'Activos', idioma: 'es', txt: 'Activos' },
    { key: 'Grupo', idioma: 'es', txt: 'Grupo' },
    { key: 'Acceso', idioma: 'es', txt: 'Acceso' },
    { key: 'Sin acceso', idioma: 'es', txt: 'Sin acceso' },
    { key: 'Modificar usuarios', idioma: 'es', txt: 'Modificar usuarios' },
    { key: 'Datos del arbol', idioma: 'es', txt: 'Datos del &aacute;rbol' },
    { key: 'Nombre', idioma: 'es', txt: 'Nombre' },
    { key: 'Idioma', idioma: 'es', txt: 'Idioma' },
    { key: 'Organizacion', idioma: 'es', txt: 'Organizaci&oacute;n' },
    { key: 'Grupo padre', idioma: 'es', txt: 'Grupo padre' },
    { key: 'Crear usuarios en grupo padre', idioma: 'es', txt: 'Crear usuarios en grupo padre' },
    { key: 'Grupos hijos', idioma: 'es', txt: 'Grupos hijos' },
    { key: 'Direccion web del grupo hijo', idioma: 'es', txt: 'Direccion web del grupo hijo' },
    { key: 'Nombre del grupo hijo', idioma: 'es', txt: 'Nombre del grupo hijo' },
    { key: 'Flores disponibles para todos los usuarios', idioma: 'es', txt: 'Flores disponibles para todos los usuarios' },
    { key: 'Condicion de consenso', idioma: 'es', txt: 'Condici&oacute;n de consenso' },
    { key: 'Crear', idioma: 'es', txt: 'Crear' },
    { key: 'Modificar', idioma: 'es', txt: 'Modificar' },
    { key: 'Mejor visto en', idioma: 'es', txt: 'Mejor visto en' },
    { key: 'simulacion', idioma: 'es', txt: 'simulaci&oacute;n' },
    { key: 'version beta', idioma: 'es', txt: 'versi&oacute;n beta' },
    { key: 'Ayuda', idioma: 'es', txt: 'Ayuda' },
    { key: 'Clave', idioma: 'es', txt: 'Clave' },
    { key: 'Actualizar', idioma: 'es', txt: 'Actualizar' },
    { key: 'Consenso', idioma: 'es', txt: 'Consenso' },
    { key: 'Si', idioma: 'es', txt: 'Si' },
    { key: 'No', idioma: 'es', txt: 'No' },
    { key: 'Datos del coordinador', idioma: 'es', txt: 'Datos del coordinador' },
    { key: 'Alta de usuario', idioma: 'es', txt: 'Alta de usuario' },
    { key: 'Entrar', idioma: 'es', txt: 'Entrar' },
    { key: 'Nombre completo', idioma: 'es', txt: 'Nombre completo' },
    { key: 'Enviar solicitud', idioma: 'es', txt: 'Enviar solicitud' },
    { key: 'Frase0', idioma: 'es', txt: 'Estamos tod@s junt@s en esto' },
    { key: 'Frase1', idioma: 'es', txt: 'Tod@s somos distint@s, pero tod@s tenemos algo en com&uacute;n.<br>Aceptamos la diversidad como parte de la vida, no la juzgamos.<br>Buscamos el lado com&uacute;n<br>y construimos sobre &eacute;l.<br>No competimos, cooperamos.' },
    { key: 'Frase2', idioma: 'es', txt: 'No competimos, cooperamos.' },
    { key: 'Frase3', idioma: 'es', txt: 'No se puede resolver los problemas<br>con la misma mentalidad con que fueron creados<br>hay que cambiar la manera de pensar primero' },
    { key: 'Frase4', idioma: 'es', txt: 'Construyamos un bonito &aacute;rbol, <br>un &aacute;rbol colectivo.' },
    { key: 'Frase5', idioma: 'es', txt: 'Tod@s triunfamos o tod@s fracasamos,<br>tod@s damos y tod@s recibimos.' },
    { key: 'Frase6', idioma: 'es', txt: 'La meta es la unidad, <br>no la unanimidad.' },
    { key: 'Frase7', idioma: 'es', txt: 'Agr&uacute;pate y conquistar&aacute;s.' },
    { key: 'Frase8', idioma: 'es', txt: 'Sincronizamos intereses, <br>tomamos decisiones, <br>generamos acciones.' },
    { key: 'Frase9', idioma: 'es', txt: 'Buscamos eficiencia, <br>no beneficio.' },
    { key: 'Frase10', idioma: 'es', txt: 'Elegimos nuestros representantes; los revocamos si es necesario.' },
    { key: 'Frase11', idioma: 'es', txt: 'No te preguntes qu&eacute; puede hacer la cooperativa por ti, <br>preg&uacute;ntate qu&eacute; puedes hacer t&uacute; por ella.' },
    { key: 'Frase12', idioma: 'es', txt: 'Bienvenid@ al juego de la cooperaci&oacute;n.' },
    { key: 'Frase13', idioma: 'es', txt: 'Una decisi&oacuten parte de elementos conocidos,<br>el resultado es pura innovaci&oacuten.' },
    { key: 'Frase14', idioma: 'es', txt: 'Si educaramos para la diversidad,<br>toda rareza ser&iacute;a bienvenida,<br>no habr&iacute;a nadie a quien juzgar,<br>y serias tan libre como puedas so&ntilde;ar.' },
    { key: 'Frase15', idioma: 'es', txt: 'Tomar una decisi&oacuten es un proceso cooperativo' },
    { key: 'Aceptar', idioma: 'es', txt: 'Aceptar' },
    { key: 'Cancelar', idioma: 'es', txt: 'Cancelar' },
    { key: 'Reiniciar', idioma: 'es', txt: 'Reiniciar' },
    { key: 'Paso', idioma: 'es', txt: 'Paso' },
    { key: 'Play', idioma: 'es', txt: 'Play' },
    { key: 'Arbol de decisiones', idioma: 'es', txt: 'Arbol de decisiones' },
    { key: 'Publicacion de argumentos', idioma: 'es', txt: 'Encuestas de evaluacion' },
    { key: 'Ambito operativo', idioma: 'es', txt: '&Aacute;mbito operativo' },
    { key: 'Documento de consenso alcanzados', idioma: 'es', txt: 'Documento de consenso alcanzados' },
    { key: 'Documentos de resultado', idioma: 'es', txt: 'Documentos de resultado' },
    { key: 'Manifiesto del grupo', idioma: 'es', txt: 'Manifiesto del grupo' },
    { key: 'Decision', idioma: 'es', txt: 'Decisi&oacute;n' },
    { key: 'Actas', idioma: 'es', txt: 'Acta' },
    { key: 'Seguimiento', idioma: 'es', txt: 'Seguimiento' },
    { key: 'Comentarios', idioma: 'es', txt: 'Argumentos' },
    { key: 'Nacido', idioma: 'es', txt: 'Nacido' },
    { key: 'Decision fecha', idioma: 'es', txt: 'Decisi&oacute;n fecha' },
    { key: 'Objetivo', idioma: 'es', txt: 'Objetivo' },
    { key: 'Revision', idioma: 'es', txt: 'Revisi&oacute;n' },
    { key: 'Integrantes', idioma: 'es', txt: 'Integrantes' },
    { key: 'Definicion', idioma: 'es', txt: 'Definici&oacute;n' },
    { key: 'Arbol', idioma: 'es', txt: '&Aacute;rbol' },
    { key: 'Borrar datos de enlace', idioma: 'es', txt: 'Borrar datos de enlace' },
    { key: 'Crear arbol', idioma: 'es', txt: 'Crear &aacute;rbol' },
    { key: 'GrupoTrabajo', idioma: 'es', txt: 'GrupoTrabajo' },
    { key: 'Estrategia', idioma: 'es', txt: 'Estrategia' },
    { key: 'Accion', idioma: 'es', txt: 'Accion' },
    { key: 'Estado', idioma: 'es', txt: 'Estado' },
    { key: 'EstadoTs', idioma: 'es', txt: 'EstadoTs' },
    { key: 'Estados', idioma: 'es', txt: 'Estados' },
    { key: 'Responsable', idioma: 'es', txt: 'Responsable' },
    { key: 'Nuevo estado', idioma: 'es', txt: 'Nuevo estado' },
    { key: 'Agregar estado', idioma: 'es', txt: 'Agregar estado' },
    { key: 'Evaluaciones', idioma: 'es', txt: 'Evaluaciones' },
    { key: 'Promedio', idioma: 'es', txt: 'Promedio' },
    { key: 'Alertas', idioma: 'es', txt: 'Alertas' },
    { key: 'Crear grupo', idioma: 'es', txt: 'Crear grupo' },
    { key: 'Datos del grupo', idioma: 'es', txt: 'Datos del grupo' },
    { key: 'Repetir clave', idioma: 'es', txt: 'Repetir clave' },
    { key: 'Nombre de arbol', idioma: 'es', txt: 'Nombre corto sin espacios ni acentos' },
    { key: 'error nombre grupo', idioma: 'es', txt: 'El nombre de grupo no puede contener<br>caracteres especiales' },
    { key: 'crearMsg', idioma: 'es', txt: 'La creaci&oacute;n de grupos requiere acompa&ntilde;amiento para comprender los conceptos y lograr una puesta en marcha exitosa. Contacte con <a target="_blank" href="http://sociocracia.net">Sociocracia.net</a> para coordinar la asistencia.' },
    { key: 'Generar', idioma: 'es', txt: 'Generar' },
    { key: 'Ver grupos', idioma: 'es', txt: 'Ver grupos' },
    { key: 'Ver personas', idioma: 'es', txt: 'Ver personas' },
    { key: 'Horizontalidad', idioma: 'es', txt: 'Horizontal' },
    { key: 'Objecion', idioma: 'es', txt: 'Objecion' },
    { key: 'Coordinador', idioma: 'es', txt: 'Coordinador' },
    { key: 'Representante', idioma: 'es', txt: 'Representante' },
    { key: 'Facilitador', idioma: 'es', txt: 'Facilitador' },
    { key: 'Finalizar seguimiento', idioma: 'es', txt: 'Finalizar seguimiento' },
    { key: 'Estructura', idioma: 'es', txt: 'Estructura' },
    { key: 'Intergrupal', idioma: 'es', txt: 'Intergrupal' },
    { key: 'Modelos de debate', idioma: 'es', txt: 'Modelos de debate' },
    { key: 'Estructuras', idioma: 'es', txt: 'Estructuras' },
    { key: 'Evaluacion de Evento', idioma: 'es', txt: 'Evaluacion de Evento' },
    { key: 'Documentos de decision', idioma: 'es', txt: 'Documentos de decisi&oacute;n' },
    { key: 'Funcion', idioma: 'es', txt: 'Funcion' },
    { key: 'nuevos modelos', idioma: 'es', txt: 'Se pueden crear nuevos modelos expecificos.<br>Contacte con <a href=\"sociocracia.net\" target=\"_blank\">Sociocracia.net</a> para analizar sus necesidades' },
    { key: 'Evaluamos', idioma: 'es', txt: 'Evaluamos' },
    { key: 'norecognition', idioma: 'es', txt: 'Este navegador no soporta reconocimiento de voz' },

    

    { key: 'Crear nuevo grupo', idioma: 'ct', txt: 'Crear nou grup' },
    { key: 'Grupos', idioma: 'ct', txt: 'Grups' },
    { key: 'auTit2', idioma: 'ct', txt: '<br /><br />L\'alta d\'usu&agrave;ries &eacute;s administrada manualment.<br /><br />Selecciona el grup<br /> i completa les teves dades.<br />Rebr&agrave;s un correu electr&ograve;nic amb les teves dades d\'acc&eacute;s.<br /><br />' },
    { key: 'ufTit1', idioma: 'ct', txt: 'Si treus la darrera flor la proposta deixar&agrave; d\'existir' },
    { key: 'Se perderan los datos no guardados', idioma: 'ct', txt: 'Es perdran les dades no desades' },
    { key: 'Nombre no puede ser vacio', idioma: 'ct', txt: 'El Nom no pot estar buit' },
    { key: 'Email no puede ser vacio', idioma: 'ct', txt: 'El Correu electr&ograve;nic no pot estar buit' },
    { key: 'Mensaje enviado al coordinador', idioma: 'ct', txt: 'Missatge enviat al coordinador' },
    { key: 'Las nuevas claves no coinciden', idioma: 'ct', txt: 'Les noves claus no coincideixen' },
    { key: 'Solo disponible para el coordinador', idioma: 'ct', txt: 'Nom&eacute;s disponible per al coordinador' },
    { key: 'Apoyos', idioma: 'ct', txt: 'Recolzaments' },
    { key: 'Ultimo', idioma: 'ct', txt: 'Darrera visita' },
    { key: 'Activo', idioma: 'ct', txt: 'Activo' },
    { key: 'Solo Lectura', idioma: 'ct', txt: 'Solo lectura' },
    { key: 'Habilitado', idioma: 'ct', txt: 'Habilitado' },
    { key: 'Secretaria', idioma: 'ct', txt: 'Secretaria' },
    { key: 'abrSoloLectura', idioma: 'ct', txt: 'Lect' },
    { key: 'abrHabilitado', idioma: 'ct', txt: 'Hab' },
    { key: 'abrCoordinador', idioma: 'ct', txt: 'Coor' },
    { key: 'abrSecretaria', idioma: 'ct', txt: 'Secr' },
    { key: 'Email', idioma: 'ct', txt: 'Correu electr&ograve;nic' },
    { key: 'Borrar', idioma: 'ct', txt: 'Borrar' },
    { key: 'No se puede borrar el usuario actual', idioma: 'ct', txt: 'No es pot borrar l\'usu&agrave;ria actual' },
    { key: 'Estructuras organizativas', idioma: 'ct', txt: 'Estructures organizatives' },
    { key: 'Cerrar', idioma: 'ct', txt: 'Tancar' },
    { key: 'Grupo padre no corresponde', idioma: 'ct', txt: 'El Grup mare no correspon' },
    { key: 'Usuarios', idioma: 'ct', txt: 'Usu&agrave;ries' },
    { key: 'Decisiones', idioma: 'ct', txt: 'Decisiones' },
    { key: 'Lunes', idioma: 'ct', txt: 'Dilluns' },
    { key: 'ene', idioma: 'ct', txt: 'gen' },
    { key: 'feb', idioma: 'ct', txt: 'feb' },
    { key: 'mar', idioma: 'ct', txt: 'mar' },
    { key: 'abr', idioma: 'ct', txt: 'abr' },
    { key: 'may', idioma: 'ct', txt: 'mai' },
    { key: 'jun', idioma: 'ct', txt: 'jun' },
    { key: 'jul', idioma: 'ct', txt: 'jul' },
    { key: 'ago', idioma: 'ct', txt: 'ago' },
    { key: 'set', idioma: 'ct', txt: 'set' },
    { key: 'oct', idioma: 'ct', txt: 'oct' },
    { key: 'nov', idioma: 'ct', txt: 'nov' },
    { key: 'dic', idioma: 'ct', txt: 'des' },
    { key: 'URL', idioma: 'ct', txt: 'LUR' },
    { key: 'conminSiPc', idioma: 'ct', txt: 'M&iacute;nimes usu&agrave;ries implicades al debat. Cada branca que parteix de l\'arrel &eacute;s un debat diferent.' },
    { key: 'conmaxNoPc', idioma: 'ct', txt: 'M&agrave;ximes usu&agrave;ries que divergeixen al debat. Si n\'hi ha m&eacute;s, llavors no s\'aconsegueix el consens.' },
    { key: 'Pausa', idioma: 'ct', txt: 'Pausa' },
    { key: 'Diverger', idioma: 'ct', txt: 'Divergir' },
    { key: 'Converger', idioma: 'ct', txt: 'Convergir' },
    { key: 'Sociocracia', idioma: 'ct', txt: 'Sociocracia' },
    { key: 'Democracia interactiva', idioma: 'ct', txt: 'Democr&agrave;cia<br>interactiva' },
    { key: 'El bosque', idioma: 'ct', txt: 'Communaut�' },
    { key: 'El arbol', idioma: 'ct', txt: 'Groupe' },
    { key: 'Mailer', idioma: 'ct', txt: 'Difusor' },
    { key: 'Dias', idioma: 'ct', txt: 'Dies' },
    { key: 'Activos', idioma: 'ct', txt: 'Actives' },
    { key: 'Grupo', idioma: 'ct', txt: 'Grup' },
    { key: 'Acceso', idioma: 'ct', txt: 'Acc&eacute;s' },
    { key: 'Sin acceso', idioma: 'ct', txt: 'Sense acc&eacute;s' },
    { key: 'Modificar usuarios', idioma: 'ct', txt: 'Modificar usu&agrave;ries' },
    { key: 'Datos del arbol', idioma: 'ct', txt: 'Dades de l\'abre' },
    { key: 'Nombre', idioma: 'ct', txt: 'Nom' },
    { key: 'Idioma', idioma: 'ct', txt: 'Llengua' },
    { key: 'Organizacion', idioma: 'ct', txt: 'Organitzaci&oacute;' },
    { key: 'Grupo padre', idioma: 'ct', txt: 'Grup mare' },
    { key: 'Crear usuarios en grupo padre', idioma: 'ct', txt: 'Crear usu&agrave;ries al grup mare' },
    { key: 'Grupos hijos', idioma: 'ct', txt: 'Grups filles' },
    { key: 'Direccion web del grupo hijo', idioma: 'ct', txt: 'Direcci&oacute; del grup filla a la xarxa' },
    { key: 'Nombre del grupo hijo', idioma: 'ct', txt: 'Nom del grup filla' },
    { key: 'Flores disponibles para todos los usuarios', idioma: 'ct', txt: 'Flors disponibles per a totes les usu&agrave;ries' },
    { key: 'Condicion de consenso', idioma: 'ct', txt: 'Condici&oacute; de consens' },
    { key: 'Crear', idioma: 'ct', txt: 'Crear' },
    { key: 'Modificar', idioma: 'ct', txt: 'Modificar' },
    { key: 'Mejor visto en', idioma: 'ct', txt: 'Millor vist en' },
    { key: 'simulacion', idioma: 'ct', txt: 'simulaci&oacute;' },
    { key: 'version beta', idioma: 'ct', txt: 'versi&oacute; beta' },
    { key: 'Ayuda', idioma: 'ct', txt: 'Ajuda' },
    { key: 'Clave', idioma: 'ct', txt: 'Clau' },
    { key: 'Actualizar', idioma: 'ct', txt: 'Actualitzar' },
    { key: 'Consenso', idioma: 'ct', txt: 'Consens' },
    { key: 'Si', idioma: 'ct', txt: 'Si' },
    { key: 'No', idioma: 'ct', txt: 'No' },
    { key: 'Datos del coordinador', idioma: 'ct', txt: 'Dades del coordinador' },
    { key: 'Alta de usuario', idioma: 'ct', txt: 'Alta d\'usu&agrave;ria' },
    { key: 'Entrar', idioma: 'ct', txt: 'Entrar' },
    { key: 'Nombre completo', idioma: 'ct', txt: 'Nom complet' },
    { key: 'Enviar solicitud', idioma: 'ct', txt: 'Enviar sol&middot;licitud' },
    { key: 'Frase0', idioma: 'ct', txt: 'Hi estem totes juntes' },
    { key: 'Frase1', idioma: 'ct', txt: 'Totes som diferentes, per&ograve; totes tenim quelcom en com&uacute;.<br>Acceptem la diversitat com a part de la vida, no la jutgem.<br>Cerquem el costat com&uacute;<br>i hi constru&iuml;m a sobre.<br>No competim, cooperem.' },
    { key: 'Frase2', idioma: 'ct', txt: 'No competim, cooperem.' },
    { key: 'Frase3', idioma: 'ct', txt: 'Els problemes no es poden resoldre<br>amb la mateixa mentalitat amb qu&egrave; van ser creats.<br>S\'ha de canviar la manera de pensar primer' },
    { key: 'Frase4', idioma: 'ct', txt: 'Constru�m un bell arbre, <br>un arbre col&middot;lectiu.' },
    { key: 'Frase5', idioma: 'ct', txt: 'Totes triunfem o totes fracassem,<br>totes donem i totes rebem.' },
    { key: 'Frase6', idioma: 'ct', txt: 'La meta &eacute;s la unitat, <br>no la unanimitat.' },
    { key: 'Frase7', idioma: 'ct', txt: 'Agrupa\'t i conquerir&agrave;s.' },
    { key: 'Frase8', idioma: 'ct', txt: 'Sincronitzem interessos, <br>prenem decisions, <br>generem accions.' },
    { key: 'Frase9', idioma: 'ct', txt: 'Cerquem efici&egrave;ncia, <br>no lucre.' },
    { key: 'Frase10', idioma: 'ct', txt: 'No necessitem representants.' },
    { key: 'Frase11', idioma: 'ct', txt: 'No et preguntis qu&egrave; pot fer la cooperativa per tu, <br>pregunta\'t qu&egrave; pots fer tu per ella.' },
    { key: 'Frase12', idioma: 'ct', txt: 'Benvinguda al joc de la cooperaci&oacute;.' },
    { key: 'Frase13', idioma: 'ct', txt: 'Un consens no es tria, es construeix.' },
    { key: 'Frase14', idioma: 'ct', txt: 'Si eduqu&eacute;ssim  per a la diversitat,<br>tota raresa seria benvinguda,<br>no hi hauria ning&uacute; qui jutjar,<br>i series tan lliure com puguis somiar.' },
    { key: 'Frase15', idioma: 'ct', txt: 'El consens &eacute;s un proc&eacute;s cooperatiu' },
    { key: 'Aceptar', idioma: 'ct', txt: 'Acceptar' },
    { key: 'Cancelar', idioma: 'ct', txt: 'Cancel' },
    { key: 'Reiniciar', idioma: 'ct', txt: 'Reiniciar' },
    { key: 'Paso', idioma: 'ct', txt: 'Pas' },
    { key: 'Play', idioma: 'ct', txt: 'Play' },
    { key: 'Arbol de decisiones', idioma: 'ct', txt: 'Arbre de decisions' },
    { key: 'Publicacion de argumentos', idioma: 'ct', txt: 'Publicacion d\'arguments' },
    { key: 'Ambito operativo', idioma: 'ct', txt: '&Agrave;mbit operatiu' },
    { key: 'Documento de consenso alcanzados', idioma: 'ct', txt: 'Document de consens aconseguits' },
    { key: 'Documentos de resultado', idioma: 'ct', txt: 'Documents de resultat' },
    { key: 'Manifiesto del grupo', idioma: 'ct', txt: 'Manifest del grup' },
    { key: 'Decision', idioma: 'ct', txt: 'Decisi&oacute;n' },
    { key: 'Actas', idioma: 'ct', txt: 'Acta' },
    { key: 'Seguimiento', idioma: 'ct', txt: 'Seguimiento' },

    { key: 'Crear nuevo grupo', idioma: 'en', txt: 'Create new group' },
    { key: 'Grupos', idioma: 'en', txt: 'Groups' },
    { key: 'auTit2', idioma: 'en', txt: '<br /><br />New users are manually controled by the gardener.<br /><br />Select a group<br /> and complete your data.<br />You will receive an email with access data.<br /><br />' },
    { key: 'ufTit1', idioma: 'en', txt: 'If you take out last flower then this propousal will drop from the tree' },
    { key: 'Se perderan los datos no guardados', idioma: 'en', txt: 'Saves data will be lost' },
    { key: 'Nombre no puede ser vacio', idioma: 'en', txt: 'Name can not be empty' },
    { key: 'Email no puede ser vacio', idioma: 'en', txt: 'Email can not be empty' },
    { key: 'Mensaje enviado al coordinador', idioma: 'en', txt: 'Messaje sent to the coordinator' },
    { key: 'Las nuevas claves no coinciden', idioma: 'en', txt: 'New passwords are different' },
    { key: 'Solo disponible para el coordinador', idioma: 'en', txt: 'Available only for the coordinator' },
    { key: 'Apoyos', idioma: 'en', txt: 'Supports' },
    { key: 'Ultimo', idioma: 'en', txt: 'Last' },
    { key: 'Activo', idioma: 'en', txt: 'Activo' },
    { key: 'Solo Lectura', idioma: 'en', txt: 'Read only' },
    { key: 'Habilitado', idioma: 'en', txt: 'Enabled' },
    { key: 'Secretaria', idioma: 'en', txt: 'Secretary' },
    { key: 'abrSoloLectura', idioma: 'en', txt: 'Read' },
    { key: 'abrHabilitado', idioma: 'en', txt: 'Ena' },
    { key: 'abrCoordinador', idioma: 'en', txt: 'Coor' },
    { key: 'abrSecretaria', idioma: 'en', txt: 'Secr' },
    { key: 'Email', idioma: 'en', txt: 'Email' },
    { key: 'Borrar', idioma: 'en', txt: 'Delete' },
    { key: 'No se puede borrar el usuario actual', idioma: 'en', txt: 'Can not remove actual user' },
    { key: 'Estructuras organizativas', idioma: 'en', txt: 'Organizational structures' },
    { key: 'Cerrar', idioma: 'en', txt: 'Close' },
    { key: 'Grupo padre no corresponde', idioma: 'en', txt: 'Parent group does not match' },
    { key: 'Usuarios', idioma: 'en', txt: 'Users' },
    { key: 'Decisiones', idioma: 'en', txt: 'Decisions' },
    { key: 'Lunes', idioma: 'en', txt: 'Monday' },
    { key: 'ene', idioma: 'en', txt: 'yan' },
    { key: 'feb', idioma: 'en', txt: 'feb' },
    { key: 'mar', idioma: 'en', txt: 'mar' },
    { key: 'abr', idioma: 'en', txt: 'apr' },
    { key: 'may', idioma: 'en', txt: 'may' },
    { key: 'jun', idioma: 'en', txt: 'jun' },
    { key: 'jul', idioma: 'en', txt: 'jul' },
    { key: 'ago', idioma: 'en', txt: 'ago' },
    { key: 'set', idioma: 'en', txt: 'sep' },
    { key: 'oct', idioma: 'en', txt: 'oct' },
    { key: 'nov', idioma: 'en', txt: 'nov' },
    { key: 'dic', idioma: 'en', txt: 'dic' },
    { key: 'URL', idioma: 'en', txt: 'URL' },
    { key: 'conminSiPc', idioma: 'en', txt: 'Minimized users involved in the debate. Each branch that starts from the root is a different debate.' },
    { key: 'conmaxNoPc', idioma: 'en', txt: 'Top users who diverge in the debate. If there is more then consensus is not reached.' },
    { key: 'Pausa', idioma: 'en', txt: 'Pause' },
    { key: 'Diverger', idioma: 'en', txt: 'Diverge' },
    { key: 'Converger', idioma: 'en', txt: 'Converge' },
    { key: 'Sociocracia', idioma: 'en', txt: 'Sociocracy' },
    { key: 'Democracia interactiva', idioma: 'en', txt: 'Interactive<br>democracy' },
    { key: 'El bosque', idioma: 'en', txt: 'Community' },
    { key: 'El arbol', idioma: 'en', txt: 'Group' },
    { key: 'Mailer', idioma: 'en', txt: 'Mailer' },
    { key: 'Dias', idioma: 'en', txt: 'Days' },
    { key: 'Activos', idioma: 'en', txt: 'Actives' },
    { key: 'Grupo', idioma: 'en', txt: 'Group' },
    { key: 'Acceso', idioma: 'en', txt: 'Access' },
    { key: 'Sin acceso', idioma: 'en', txt: 'No access' },
    { key: 'Modificar usuarios', idioma: 'en', txt: 'Modify users' },
    { key: 'Datos del arbol', idioma: 'en', txt: 'Tree data' },
    { key: 'Nombre', idioma: 'en', txt: 'Name' },
    { key: 'Idioma', idioma: 'en', txt: 'Lang' },
    { key: 'Organizacion', idioma: 'en', txt: 'Organization' },
    { key: 'Grupo padre', idioma: 'en', txt: 'Parent group' },
    { key: 'Crear usuarios en grupo padre', idioma: 'en', txt: 'Create users in parent group' },
    { key: 'Grupos hijos', idioma: 'en', txt: 'Children groups' },
    { key: 'Direccion web del grupo hijo', idioma: 'en', txt: 'Wed address of son group' },
    { key: 'Nombre del grupo hijo', idioma: 'en', txt: 'Name of son group' },
    { key: 'Flores disponibles para todos los usuarios', idioma: 'en', txt: 'Available flowers for all users' },
    { key: 'Condicion de consenso', idioma: 'en', txt: 'Consensus condition' },
    { key: 'Crear', idioma: 'en', txt: 'Create' },
    { key: 'Modificar', idioma: 'en', txt: 'Modify' },
    { key: 'Mejor visto en', idioma: 'en', txt: 'Best seen in' },
    { key: 'simulacion', idioma: 'en', txt: 'simulation' },
    { key: 'version beta', idioma: 'en', txt: 'beta version' },
    { key: 'Ayuda', idioma: 'en', txt: 'Help' },
    { key: 'Clave', idioma: 'en', txt: 'Password' },
    { key: 'Actualizar', idioma: 'en', txt: 'Refresh' },
    { key: 'Consenso', idioma: 'en', txt: 'Consensus' },
    { key: 'Si', idioma: 'en', txt: 'Yes' },
    { key: 'No', idioma: 'en', txt: 'No' },
    { key: 'Datos del coordinador', idioma: 'en', txt: 'Gardener data' },
    { key: 'Alta de usuario', idioma: 'en', txt: 'New user' },
    { key: 'Entrar', idioma: 'en', txt: 'Get in' },
    { key: 'Nombre completo', idioma: 'en', txt: 'Complete name' },
    { key: 'Enviar solicitud', idioma: 'en', txt: 'Send request' },
    { key: 'Frase0', idioma: 'en', txt: 'We are all in this' },
    { key: 'Frase1', idioma: 'en', txt: 'We are all different, but we all have something in common. <br> We accept diversity as part of life, we do not judge it. <br> We seek the common side <br> and we build over it.<br> We do not compete, we cooperate.' },
    { key: 'Frase2', idioma: 'en', txt: 'We do not compete, we cooperate.' },
    { key: 'Frase3', idioma: 'en', txt: 'You can not solve problems with the same mentality with which they were created <br> you have to change the way of thinking first' },
    { key: 'Frase4', idioma: 'en', txt: 'Let\'s build a nice tree, <br> a collective tree.' },
    { key: 'Frase5', idioma: 'en', txt: 'We all succeed or we all fail, we all give and we all receive' },
    { key: 'Frase6', idioma: 'en', txt: 'The goal is unity,<br> not unanimity.' },
    { key: 'Frase7', idioma: 'en', txt: 'Group and conquer.' },
    { key: 'Frase8', idioma: 'en', txt: 'We synchronize interests, <br> we make decisions, <br> we generate actions.' },
    { key: 'Frase9', idioma: 'en', txt: 'We seek efficiency, <br>not profit.' },
    { key: 'Frase10', idioma: 'en', txt: 'We do not need representatives.' },
    { key: 'Frase11', idioma: 'en', txt: 'Do not ask yourself what can do the cooperative for you, <br> ask yourself what can you do for the cooperative.' },
    { key: 'Frase12', idioma: 'en', txt: 'Welcome to the cooperation game.' },
    { key: 'Frase13', idioma: 'en', txt: 'A consensus is not chosen, it is built.' },
    { key: 'Frase14', idioma: 'en', txt: 'If we will educate for diversity, <br> all rarity will be welcome, <br> there will be no one to judge, <br> and you will be as free as you can be dream.' },
    { key: 'Frase15', idioma: 'en', txt: 'Consensus is a cooperative process' },
    { key: 'Aceptar', idioma: 'en', txt: 'Accept' },
    { key: 'Cancelar', idioma: 'en', txt: 'Cancel' },
    { key: 'Reiniciar', idioma: 'en', txt: 'Reset' },
    { key: 'Paso', idioma: 'en', txt: 'Step' },
    { key: 'Play', idioma: 'en', txt: 'Play' },
    { key: 'Arbol de decisiones', idioma: 'en', txt: 'Decision tree' },
    { key: 'Publicacion de argumentos', idioma: 'en', txt: 'Publication of arguments' },
    { key: 'Ambito operativo', idioma: 'en', txt: 'Operating environment' },
    { key: 'Documento de consenso alcanzados', idioma: 'en', txt: 'Consensus documents' },
    { key: 'Documentos de resultado', idioma: 'en', txt: 'Result documents' },
    { key: 'Manifiesto del grupo', idioma: 'en', txt: 'Group Manifest' },
    { key: 'Crear grupo', idioma: 'en', txt: 'Group Manifest' },
    { key: 'Datos del grupo', idioma: 'en', txt: 'Group Manifest' },
    { key: 'Repetir clave', idioma: 'en', txt: 'Group Manifest' },
    { key: 'Decision', idioma: 'en', txt: 'Decision' },
    { key: 'Actas', idioma: 'en', txt: 'Minutes' },
    { key: 'Seguimiento', idioma: 'en', txt: 'Tracing' }
];

function tr(key) {
    //aplico traduccion

    var trIdioma = navigator.language || navigator.userLanguage;

    if (idioma && idioma != '')
        trIdioma = idioma.toLowerCase();

    if (dictionary) {
        for (var i in dictionary) {
            var entry = dictionary[i];
            if (entry.key.toLocaleLowerCase() == key.toLocaleLowerCase() && entry.idioma == trIdioma)
                return entry.txt;
        }
        return '[' + key + ']';
    }
    else
        return '[' + key + ']';
}